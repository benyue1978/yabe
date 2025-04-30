using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using System.IO.BACnet; // Reference Yabe.Core
using System.Collections.Generic;
using Avalonia.Threading;
using Yabe.Avalonia;

namespace Yabe.Avalonia
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        public ObservableCollection<DeviceViewModel> Devices { get; } = new ObservableCollection<DeviceViewModel>();
        public ICommand DiscoverCommand { get; }
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        private BacnetClient? _client;
        private HashSet<uint> _foundDeviceIds = new HashSet<uint>();
        private Dictionary<uint, DeviceViewModel> _deviceMap = new();

        private DeviceViewModel? _selectedDevice;
        public DeviceViewModel? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged();
                if (value != null)
                {
                    Log($"Loading object tree for device {value.Id}...");
                    if (value.BacnetAddress != null)
                    {
                        Task.Run(() => LoadDeviceObjectsAsync(value, value.BacnetAddress));
                    }
                    else
                    {
                        Log($"Cannot get device address: {value.Address}");
                    }
                }
            }
        }

        private ObjectViewModel? _selectedObject;
        public ObjectViewModel? SelectedObject
        {
            get => _selectedObject;
            set { _selectedObject = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            DiscoverCommand = new RelayCommand(async () => await DiscoverDevicesAsync());
        }

        private void Log(string msg)
        {
            var line = $"[{DateTime.Now:HH:mm:ss}] {msg}";
            Dispatcher.UIThread.Post(() => Logs.Insert(0, line));
        }

        private async Task DiscoverDevicesAsync()
        {
            Devices.Clear();
            _foundDeviceIds.Clear();
            _deviceMap.Clear();
            SelectedDevice = null;
            Logs.Clear();
            Log("Starting device discovery...");
            try
            {
                if (_client != null)
                    _client.OnIam -= OnIam;
                _client?.Dispose();
                _client = new BacnetClient(0xBAC0, 2000, 3);
                _client.OnIam += OnIam;
                _client.Start();
                _client.WhoIs();
                Log("Broadcast WhoIs");
                await Task.Delay(2000);
                if (_client != null)
                    _client.OnIam -= OnIam;
                _client.Dispose();
                _client = null;
                Log($"Discovery complete, found {_foundDeviceIds.Count} devices");
            }
            catch (Exception ex)
            {
                Log($"Discovery exception: {ex.Message}");
            }
        }

        private void OnIam(BacnetClient sender, BacnetAddress adr, uint device_id, uint max_apdu, BacnetSegmentations segmentation, ushort vendor_id)
        {
            try
            {
                if (_foundDeviceIds.Contains(device_id)) return;
                _foundDeviceIds.Add(device_id);
                Log($"Received IAm: ID={device_id} Address={adr} Vendor={vendor_id}");
                var dev = new DeviceViewModel
                {
                    Id = device_id,
                    Address = adr.ToString(),
                    Vendor = vendor_id.ToString(),
                    Display = $"ID={device_id} Address={adr} Vendor={vendor_id}",
                    BacnetAddress = adr
                };
                _deviceMap[device_id] = dev;
                Dispatcher.UIThread.Post(() => Devices.Add(dev));
                // Asynchronously read device name, object count, etc.
                Task.Run(() => LoadDeviceDetails(dev, adr));
            }
            catch (Exception ex) { Log($"IAm processing exception: {ex.Message}"); }
        }

        private void LoadDeviceDetails(DeviceViewModel dev, BacnetAddress adr)
        {
            try
            {
                using var client = new BacnetClient(0xBAC0, 2000, 3);
                client.Start();
                // Read name
                try
                {
                    if (client.ReadPropertyRequest(adr, new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, dev.Id), BacnetPropertyIds.PROP_OBJECT_NAME, out var nameList))
                    {
                        dev.Name = nameList.Count > 0 ? nameList[0].Value?.ToString() ?? "" : "";
                        Log($"Device {dev.Id} read name: {dev.Name}");
                    }
                }
                catch (Exception ex) { Log($"Device {dev.Id} read name failed: {ex.Message}"); }
                // Read object list
                try
                {
                    if (client.ReadPropertyRequest(adr, new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, dev.Id), BacnetPropertyIds.PROP_OBJECT_LIST, out var objList))
                    {
                        dev.ObjectCount = objList.Count.ToString();
                        Log($"Device {dev.Id} object count: {dev.ObjectCount}");
                    }
                }
                catch (Exception ex) { Log($"Device {dev.Id} read object list failed: {ex.Message}"); }
                // Read vendor name
                try
                {
                    if (client.ReadPropertyRequest(adr, new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, dev.Id), BacnetPropertyIds.PROP_VENDOR_NAME, out var vendorNameList))
                    {
                        dev.Vendor = vendorNameList.Count > 0 ? vendorNameList[0].Value?.ToString() ?? dev.Vendor : dev.Vendor;
                        Log($"Device {dev.Id} vendor: {dev.Vendor}");
                    }
                }
                catch (Exception ex) { Log($"Device {dev.Id} read vendor failed: {ex.Message}"); }
                Dispatcher.UIThread.Post(() => dev.NotifyAll());
            }
            catch (Exception ex) { Log($"Device {dev.Id} details read exception: {ex.Message}"); }
        }

        public async Task LoadDeviceObjectsAsync(DeviceViewModel dev, BacnetAddress adr)
        {
            try
            {
                using var client = new BacnetClient(0xBAC0, 2000, 3);
                client.Start();
                // Read object list
                if (client.ReadPropertyRequest(adr, new BacnetObjectId(BacnetObjectTypes.OBJECT_DEVICE, dev.Id), BacnetPropertyIds.PROP_OBJECT_LIST, out var objList))
                {
                    dev.Objects.Clear();
                    foreach (var obj in objList)
                    {
                        if (obj.Value is BacnetObjectId objId)
                        {
                            var objVm = new ObjectViewModel { ObjectId = objId, Type = objId.type.ToString() };
                            // Read object name
                            if (client.ReadPropertyRequest(adr, objId, BacnetPropertyIds.PROP_OBJECT_NAME, out var nameList))
                                objVm.Name = nameList.Count > 0 ? nameList[0].Value?.ToString() ?? "" : "";
                            // Read common properties
                            var propListToRead = YabeObjectPropertyLoader.GetProperties(objId.type);
                            foreach (BacnetPropertyIds pid in propListToRead)
                            {
                                if (client.ReadPropertyRequest(adr, objId, pid, out var propList))
                                {
                                    foreach (var prop in propList)
                                    {
                                        objVm.Properties.Add(new PropertyViewModel { Name = pid.ToString(), Value = prop.Value?.ToString() ?? "" });
                                    }
                                }
                            }
                            dev.Objects.Add(objVm);
                        }
                    }
                    Log($"Device {dev.Id} object browser completed, {dev.Objects.Count} objects");
                }
            }
            catch (Exception ex) { Log($"Device {dev.Id} object browser exception: {ex.Message}"); }
        }
    }

    public class DeviceViewModel : NotifyPropertyChangedBase
    {
        public uint Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        public string Vendor { get => _vendor; set { _vendor = value; OnPropertyChanged(); } }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string ObjectCount { get => _objectCount; set { _objectCount = value; OnPropertyChanged(); } }
        public string Display { get => _display; set { _display = value; OnPropertyChanged(); } }
        public ObservableCollection<ObjectViewModel> Objects { get; } = new();
        public BacnetAddress? BacnetAddress { get; set; }
        private uint _id;
        private string _address = "";
        private string _vendor = "";
        private string _name = "";
        private string _objectCount = "";
        private string _display = "";
        public void NotifyAll() { OnPropertyChanged(nameof(Id)); OnPropertyChanged(nameof(Address)); OnPropertyChanged(nameof(Vendor)); OnPropertyChanged(nameof(Name)); OnPropertyChanged(nameof(ObjectCount)); OnPropertyChanged(nameof(Display)); }
    }

    public class ObjectViewModel : NotifyPropertyChangedBase
    {
        public BacnetObjectId ObjectId { get => _objectId; set { _objectId = value; OnPropertyChanged(); } }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Type { get => _type; set { _type = value; OnPropertyChanged(); } }
        public ObservableCollection<PropertyViewModel> Properties { get; } = new();
        private BacnetObjectId _objectId;
        private string _name = "";
        private string _type = "";
        public void NotifyAll() { OnPropertyChanged(nameof(ObjectId)); OnPropertyChanged(nameof(Name)); OnPropertyChanged(nameof(Type)); }
    }

    public class PropertyViewModel : NotifyPropertyChangedBase
    {
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Value { get => _value; set { _value = value; OnPropertyChanged(); } }
        private string _name = "";
        private string _value = "";
    }

    // Base class supporting property notifications
    public class NotifyPropertyChangedBase : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    // Async command implementation
    public class RelayCommand : ICommand
    {
        private readonly Func<Task>? _executeAsync;
        private readonly Action? _execute;

        public RelayCommand(Action execute) => _execute = execute;
        public RelayCommand(Func<Task> executeAsync) => _executeAsync = executeAsync;

        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public async void Execute(object? parameter)
        {
            if (_executeAsync != null)
                await _executeAsync();
            else
                _execute?.Invoke();
        }
    }
} 
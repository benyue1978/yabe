-------------------------------------------------------------------------------
                   YABE - YET ANOTHER BACNET EXPLORER
-------------------------------------------------------------------------------

1.  INTRO

	1.1 ABOUT
		Yabe is a graphical windows program for exploring and navigating BACnet
		devices. 
		The project was created in order to test and evaluate the BACnet protocol.
		In specific I needed a platform on which to test theoretical performance 
		throughput for huge amounts of data over MSTP/RS485 multidrop network. 
		There're other free/open programs that also implements BACnet "exploring". 
		So far I haven't found any to my satisfaction though. Some are worth 
		mentioning. 
		InneaBACnetExplorer:
		http://www.inneasoft.com/index.php/en/products/products-bacnet/products-bacnet-explorer.html
		A very nice explorer program. Same concept as Yabe. (Perhaps even better
		than Yabe.) However if you want to do basic things like "write" or 
		"subscribe" to a node, you have to pay a cartful of money. Also it only 
		supports BACnet/IP and no segmentation. 
		BACnet Stack - By Steve Karg:
		http://bacnet.sourceforge.net/
		A very nice old open source ANSI C project with a lot of console based 
		sample programs. Implements both BACnet/IP and BACnet/MSTP. Steve is very 
		active and famous in the BACnet world. The code is "singleton" based 
		though. Meaning that the code is only able to function as either BACnet/IP
		or BACnet/MSTP and only 1 connection is supported. This is fine for a MCU
		device but not very useful on devices like PCs. 
		I would have liked to contribute and base my tests on Steves project, but
		the shift to "session" based code (as agreed upon in the mail list) may 
		have been too big a mouthful. My patches were not approved. 
		CAS BACnet Explorer, by Chipkin Automation Systems:
		http://www.chipkin.com/products/software/bacnet-software/cas-bacnet-explorer
		Same concept as Yabe. Not as nicely finished as InneaBACnetExplorer and 
		not as comprehensive as BACnet Stack. To be honest, the program is 
		actually fairly horrible. It runs very poorly (at least on my machine) and
		the interface is not much different from a console. The documentation are 
		very nice though. Also got a few articles about performance in MSTP.
	
		This document is subject to change.

	1.2 SEGMENTATION
		I've implemented 'segmentation' as a part of my performance testing. To
		see it in action, I've added a 'HugeBlob' octet value in the DemoServer, 
		ranging 2000 bytes. This is more than the max_adpu. 
		The file operations are also able to use 'segmentation' if the options are
		set accordingly.
		I'm not sure if my implementation or my usage is correct though. I've 
		followed the guidelines from the 'standard'. But my copy is rather old and 
		I haven't found any other BACnet programs that supports it. 
		I also support 'segmentation' with a window_size > 1 in MSTP. (This was my
		original purpose.) According to my 'standard' this is illegal. I hope that
		means that my copy is old. I was recommended to this solution by the 
		bacnet-l mailing list. 

	1.3 CREDITS
		The projected is created by Morten Kvistgaard, anno 2014. 
		F. Chaxel has contributed a lot of the later additions (eg Foreign Device
		Registration, BBMD services, TrendLog & Schedule display, Calendar editor,
		Alarms summary, BACnet on Ethernet, BACnet IPv6, BACnet Secure Connect).
		A few patches and input has been given by the community.
		Graphics are the usual FamFamFam: http://www.famfamfam.com/
		Serializing (most/some) is ported from project by Steve Karg:
		http://bacnet.sourceforge.net/
		GUI and concept is inspired by UaExpert:
		http://www.unified-automation.com/products/development-tools/uaexpert.html
		Zedgraph come from http://sourceforge.net/projects/zedgraph/
		Calendar control come from :
		http://www.codeproject.com/Articles/38699/A-Professional-Calendar-Agenda-View-That-You-Will
		Sharppcap come from : http://sourceforge.net/projects/sharppcap/
		Websocket-sharp come from : https://github.com/sta/websocket-sharp

2.  USAGE

	2.1 BACNET/IP OVER UDP
		- Start up the DemoServer program or another BACnet device.
		- Start Yabe.
		- Select "Add device" under "Functions".
		- Press the "Start" button in the "BACnet/IP over Udp" field.
		  The program will now add a Udp connection to the "Devices" tree and send
		  out some "WhoIs" broadcasts. If there're any BACnet/IP devices in the 
		  network they will show up in the tree. The DemoServer will show up as 
		  something like "192.168.1.91:57049 - 389002". This is the IP, the Udp
		  port and the device_id.
		- If you have more than 1 ethernet card, you can also select a local 
		  endpoint ip. (Before you click the "Add"). Either select one from 
		  the list or write one by hand, if the interface doesn't have a gateway. 
		- In order to explore the device address space, click on the device in
		  the "Devices" tree.
		  The program will fetch all "registers" or "nodes" from the device and
		  display them in the "Address Space" tree.
		- In order to explore the properties for a given register, select the node
		  in the "Address Space" tree.
		  The program will fetch all properties and display them in the 
		  "Properties" field. 
		- Write a given property by clicking the given field and insert a 
		  new value. The value will be written and read back. Be aware that many 
		  properties cannot be written. The device will decide that.
		- Subscribe to a given register/node by draging the node from the 
		  "Address Space" tree to the "Subscriptions" field.
		- Download or upload a file to the device by right clicking a file node
		  in the "Address Space" tree and select "Upload File" or "Download File". 
		- To send out a new "WhoIs" right click the transport in the "Devices" tree
		  and select "WhoIs".

	2.2 BACNET/IPv6 over UDP
		- Experimental NOT TESTED with third party device or software.
		- Start Yabe.
		- In the Options/settings menu set IPv6_support to true
		- Do the same as explain in 2.1, but the Local endpoint combo box shows now
		  IpV4 & IPv6 available interfaces. Choose an IPv6 one (the [FE80::...] will
		  be generaly OK, or simply try [::] ). If the combo is empty it's equal to
		  the default IPv4 address !
		- If the option YabeDeviceId is -1, a random VMac (with duplication test)
		  is used.
		- Register as a foreign device & BBMD services also working on IPv6. In the
		  Foreign Device Registry form one can give an IPv6 remote address or also
		  the host name.

		- Feedback is very welcome.

	2.3 BACNET/MSTP
		- For general usage refer to section 2.1. 
		- In the "Search" dialog select a COM port in the port combo box and press
		  "Start". 
		  Notice that the "Source Address" defaults to "-1". This is not a valid 
		  address and you will not be able to communicate with the device through 
		  this. The program will still be able to listen in on the network though.
		  The program will now list all the devices that communicates on the 
		  network. And it will also display all "PollForMaster" destinations that
		  goes unanswered. This way you'll be able to determine what source_address
		  to give to the program. 
		- Click on the device node in the "Devices" tree. If the source_address is
		  configured to "-1" the program will ask if you will define a new one.
		  You must do so, in order to continue communication. 
	  
	2.4	BACNET/PTP
		- For general usage refer to section 2.1. 
		- In the "Search" dialog select a COM port in the port combo box and press
		  "Start". 
		  The BACnet/PTP transport is meant for 1-to-1. Eg. RS232 or ... usb? So
		  far I haven't found any others easy accessible tools that also supports
		  it. So I haven't been able to test it. It's implemented purely by doc.

	2.5	BACNET/ETHERNET
		- pcap/winpcap must be installed on the Pc before usage
		       go to http://www.tcpdump.org/ or https://www.winpcap.org/
		       or more simply download wireshark https://www.wireshark.org/#download
		- For general usage refer to section 2.1.
		- Select an Ethernet or Wifi Interface
		- Press the "Start" button in the "BACnet/Ethernet" field.

	2.6	BACNET/SC
		- For general usage refer to section 2.1.
		- A configuration file is required and can be edited for parameters such as 
		  the remote URI, the unique 'extra long' application ID and TLS security data
		  (Yabe client certificate with private key, remote device or Hub certificate).
		  The optional certificat password is not saved in the file. The value has to
		  be given each time.
		  The remote devices certificates or CAs file could be a concatenation of several 
		  certificates in a unique .pem file. It's not required if it share the same direct 
		  CA as Yabe. 
		- Non standard uncyphered, unauthenticated ws:// can be use for test.
		- Cyphered communication can be forwarded on loopback port for Wireshark traffic
		  analysis. In order to detect BACnet/SC protocol Wireshark should listen first.
		  Value -1 to disable the service.
		- Today on Windows 10 TLS1.3 is not operational by default. You should
		  configure the system to accept it.
		  Jump to Yabe source code trunk\Docs, get the "ActivateTLS1.3 On Win10.reg"  
		  file and double click on it.
		  This will have no effect on any versions of Windows prior to 1903, and no more
		  working on version 22H2 !

	2.7 OPTIONS
	A few selected options.

		2.7.1 Udp_ExclusiveUseOfSocket
			Set this to 'true' to force single socket usage on port 0xBAC0. A value of 
			'false' will create an extra unicast socket and allow multiple clients on
			same ip/machine.

		2.7.2 Subscriptions_Lifetime
			Subscriptions will be created with this lifetime. Eg. after 120 seconds the
			subscription will be removed by device. Set to 0 to disable.
	
		2.7.3 Subscriptions_IssueConfirmedNotifies
			By default notifications will be sent 'unconfirmed'. If you think your 
			notifications are important set this to 'true' instead. 

		2.7.4 MSTP_DisplayFreeAddresses
			By default a MSTP connection will display all 'free' addresses in the 
			'Device' tree. This can help select a source_address for the program.
			If you don't want to see the 'free' entries, set this option to 'false'

		2.7.5 MSTP_LogStateMachine
			The MSTP code is able to display all state changes in log. This is very
			verbose. It may help you understand the MSTP better though.
	
		2.7.6 Segments_Max
			This value sets 'allowed max_segments' to send to the client. The client
			might not support segmentation though. If it gives you trouble, set this 
			to 0 to disable.
	
		2.7.7 DefaultDownloadSpeed
			This value sets the method for 'file download'. (This is part of the 
			original tests.) 
			The default value of '0' will result in a standard 'send request, wait 
			for response' sequence. This is rather efficient on udp.
			Value '1' will result in a 'stacked asynchronous' sequence. This is suited
			for MSTP when combined with increasing the max_info_frames. 
			Value '2' will result in a 'segmented' sequence. This is the most efficient
			for both Udp and MSTP. This is the result I sought!
	
		2.7.8 Udp_DontFragment
			This will enforce (if set to 'true') no fragmentation on the udp. It ought
			to be enforced, but it turns out that MTU is a bit tricky. (See 2.5.9)

		2.7.9 Udp_MaxPayload
			The max payload for udp seems to differ from the expectations of BACnet.
			The most common payload is 1472. Which is 1500 when added with the 28 bytes
			ip headers. This number is determined by your local switch/router through.

		2.7.10 DefaultPreferStructuredView
			The Addendum 135d defines a 'Structured View' entry in the address space.
			This enables a hierarchical address space. (Thank you very much.)
			Though if you like the flat model better, set this to 'false'.

		2.7.11 DefaultWritePriority
			Priorty level used for write operation. Can be changed without reboot.
			<Ctrl><Alt> + 0 to 9 keys are shortcuts to change this value directly 
			from the main form (0: no priority to priority level 9, for others ... 
			no shortcut): a sound is played.

		2.7.12 YabeDeviceId
			If this value is positive Yabe send response to Who-Is with this Bacnet
			device Id. Can be usefull to set recipients list in notification class 
			objects without using Yabe Ip endpoint. 

		2.7.13 DisplayIdWithName
			Leaves properties Id (such as ANALOG_INPUT:0) along with the properties 
 			name or hides this Id.

		2.7.14 Address space view & Properties
			The Addendum 135d defines a 'Structured View' entry in the address space. 
			This enables a hierarchical address space (selection 'Structured'). 
			Though if you like the flat model better, set this to 'List'. 
			The option both combines both display modes. 
			FieldTechnician view is a flat view with filter described in the file 
			SimplifiedViewFilter.xml.
			This Simplified view can be activated/desactivated with the shortcut 
			<Ctrl><Alt> + S

		2.7.15 Expand properties
			Array and structure can be atomaticaly expanded using two parameters
			"Auto Expand Grid Array Max Size" & "Always Expanded Properties".
			The first one opens all Array with a size inferior to the value.
			The second always opens Array and structure given in the comma 
			separated String, even with a hugh size.
			0 and an empty String are accepted.

		2.7.16 Plugins
			List of plugins to be loaded (see  2.9).

		2.7.17 Network View & Device Class View
			Devices are by default displayed according to the network structure.
			A "User" configurable view can also be added with hierachical Folders.
			This is done using three parameters in the settings GUI section.
			"Device Mode View" can change the mode. "Not Affected" is the 
			intial name of the folder where unknown devices are placed. If the name
			is changed to blank, it will be hidden (for a well-known network).
			"Device Class Structure" describe the hierarchy to use such as :
			HVAC(3,9);Lighting(9,23);Building(HVAC,Lighting,40,27)
			HVAC, Lighting, ... are folders. When device 9 is discovered, it is
			added to two locations while device 3 is only placed in the HVAC folder.
			The two folders are in a main folder named Building where devices
			40 and 27 are also placed. Finally all others devices (eg 500) are
			placed in the special folder 'Not Affected'.
			Re-organisation can be done with Drag/Drop operations by moving
			Folders in Folder, and Devices in Folder.
			A menu View Folders can be used to delete/rename/insert Folder.
			To experience this mode just change "Device Mode View" and restart.

		2.7.18 Background Operations
			On IAm reception Yabe can automatically send queries to get back the
			object dictionary. Two parameters in the General Settings section can
			be used to activate it. It's network consuming. Restart required.
			Do not use more than 1 thread on direct network other than BACnet/IP.

	2.8 Bacnet Object name
			By default Bacnet objects are displayed using the object identifier eg : 
			ANALOG_INPUT:0, DEVICE:333 ...
			During network exploration, when object properties are read this
			ANALOG_INPUT:0 is replaced by the value present in the PROP_NAME field
			eg : Outdoor_Temperature. The id always appears in the help tooltip.

			This mapping between identifier and name can be rendered persistent
			over Yabe sessions, using the 'Object Names database' menus.
			By default this behaviour is not active. It can also be desactivated
			if enabled by setting an empty string in the option parameter
			'ObjectNameFile' instead of a file reference.
			In order to enable the option, just go to the menu 'Save As' and choose
			a new file somewhere on your disk. That's all. Automatic saving of new
			explored name is done when closing Yabe.
			It doesn't matter if objects are created or deleted, if name are changed,
			Yabe changes it's database each time read operations are perform on objects.

			'Open' can be used to open a particular data file (do it just after Yabe is
			started, before any other operation, it's better), this file become the 
			current one, and new devices/objects are added into it.
			'Clean' can be used to remove all values.
			'Save As' can be used to create a new file (after a clean operation maybe),
			this new file become the current one.
			
			If Yabe is used on severals differents networks take care to use a file
			for each one.
			When a physical device address is modified, for Yabe it apears as a new 
			one, and the old object mapping will never be cleanned in the file.
			If a device got a physical address of an old one, the mapping is wrong, but
			after all read operations in the Address space, it's OK.

	2.9 External plugins
			User plugins written en C# can be add to Yabe (Menu Option-Plugins). 
			See Readme file in the YabePlugins source code directory.

	2.10 COV, Event logging & live Graph
			Objects can be drag/drop on the central panel to be subscribed.
			If the drag/drop is done onto the graph the value is also drawn.
			Subscriptions updates & Events displayed in the central panel can be 
			logged into a CSV file : right click in this central panel to do it.

			To remove an entry select it then hit the delete key.

			A file with a list can be also provided : see AdvertiseSample.cov.
			
3.  TECHNICAL
    
	3.1 MULTIPLE UDP CLIENTS ON SAME IP
		Most BACnet/IP programs are not able to coexist on a single PC.
		This is because they only connect to the one BACnet udp port 
		and seeks to do all their traffic through this. This will work only if you 
		got max 1 client at each machine. Eg. you'll need a virtual machine if you 
		want to work with these. I cannot find anywhere in the standard that 
		defines that you have to do it this way. I could be mistaken though. 
		A better way to solve this, is to connect 2 sockets in your program 
		instead. The BACnet socket at 0xBAC0 should be used for receiving 
		broadcasts. Not to transmit anything or receive any unicasts. If you want 
		to transmit a broadcast you use the other socket (a random port given to 
		you by the PC) and send the broadcast to the 0xBAC0 port. Your random port
		will be flagged as the source for this broadcast. When other clients pick
		up the broadcast they will transmit back their unicast to this random port
		instead of the 0xBAC0 port. This way everything works.
		This is a common way to do it.

	3.2 MSTP TIMINGS
		The timing requirements in the BACnet/MSTP standard is a bit tricky issue
		on systems like Windows. In particular the standard defines a time slot in
		which to assume ownership of the token. The time slot is defined as 
		[500 + TS*10 : 500 + (TS+1)*10] ms. This is a 10 ms time slot measured 
		after the last byte on the line. 
		This code implements the MSTP Master state machine a bit differently from the
		standard. The standard is designed for small unscheduled MCUs. Scheduled 
		systems like RTOS and Windows can take advantage of blocking code structure
		and thereby give a bit more simple code. This is probably not wise to use 
		in non real time systems like Windows. But so I've done. The first version
		was implemented with a high resolution timer (The Stopwatch class in .NET)
		but I like this version better. And it still retains most of the state
		machine.

4.  TESTS
	The DemoServer and Yabe has been tested with other similar programs:
		- InneaBACnetExplorer
		- BACnet Stack - By Steve Karg
		- Wireshark. (This is most likely the same as the BACnet Stack)
		- CAS BACnet Explorer
	'Segmentation' has been verified with Wireshark, and Wago 750/830
	BACNET/PTP has not been tested with any 3rd parties.
		- please HELP !
    	BACnet/MSTP has been tested with an Ftdi Usb/Rs485 adaptor and
	    	- Trane Uc800 (vendor Id 2)
		- Metz Connect I/O modules (BTR Netcom vendor Id 421)
		- Schneider Electric SE8350 (vendor Id 10)
		- Contemporary Control MSTP/IP Router (vendor Id 245)
		- and much more
	BACnet/Ethernet has been tested with
		- Delta Controls devices (vendor Id 8)
	BACnet/IP has been tested with certainly all devices on the market
	BBMD services has been tested with peers :
		- Wago 750/830 (vendor Id 222)
		- Newron DoGate (vendor Id 451)
		- Sauter EY-AS521 (vendor Id 80)
	BACnet/SC has been tested with
		- BACnet Reference Stack tools
 		- ScadaEngine BACnet Simulator
 		- MBS Hub/Router device in the field of BACnet/SC plugfest plateform.

5.  SUPPORT
	There's no support for the project at this time. 
	If you write to me, I'm unlikely to answer. 

6.  REPORT ERRORS
	Yeh, there be errors alright. There always are. Many won't be interesting
	though. Eg. if you find a computer that behaves differently from others, 
	I'm unlikely to care. This is not a commercial project and I'm not trying 
	to push it to the greater good of the GPL world. (This may change though.)
	If you find a device that doesn't work with Yabe, it might be interesting.
	But in order for me to fix it, I need either access to the physical device
	or printouts from programs like Wireshark, that displays the error.
	Write to us at fchaxel@free.fr or mk@pch-engineering.dk.

7.  CONTRIBUTE
	Really? You think it's missing something? It's not really meant as a huge 
	glorified project you know, but if you really must, try contacting 
	fchaxel@free.fr.
	
8.  MISC
	Project web page is located at: 
	https://sourceforge.net/projects/yetanotherbacnetexplorer/

9.  CONTRIBUTORS, in order of apearance
	Morten Kvistgaard	2014-2016	PCH Engineering/Danemark
	Frédéric Chaxel		2015-2024	University of Lorraine/France
	Adam Guzik		2015		Quark Communications, Inc/USA
	Christopher Günther	2015, 2019
	Thamer Al-Salek		2016
	Alexander Jaszkowski	2016-2024	Sauter/Germany
	Lance Tollenaar		2022-2024	New Zealand
	Frank Schubert		2022-2024	Beckhoff Automation/Germany 
	Marco Diekmann		2024		Beckhoff Automation/Germany
	Daniel Evers		2024		Bosch/Germany

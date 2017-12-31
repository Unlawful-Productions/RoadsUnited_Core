This file can be used to post ideas or planned updates. Figure sharing info would help to share pending updates.

Unlawful:
Going to make some heavy revisons in the core mod file.
   I finally finished the damn texture maps. Back to fun times.

   New texture naming convention, will eliminate any confusion with regards to texture generation abd
also combine NExt roads with the rest to make things easier. The if(x.exists(yyy.zzz)) checks take care of that.
This came about as a need to mass produce textures with simple names. Real  keyboard macros and the 
Bulk Renaming Utility.

//Killface
I just used Photoshop and Illustrator, no need for mass producing simple names or macros or bulk renaming. I also uploaded those source files.
So I don't see a need for new naming conventions. This mod was designed to replace the vanilla textures by name 1:1, with only exceptions made to extras added by the mod. Exceptions shouldn't become the standard. I have reworked all my PSD files which I was planing to use as a base for future texture packs to automatically generate all graphics needed - which works perfectly. There is no need for me to fiddle manually with the file names and risk faulty replacements.

Also I think it's a bad idea mixing up the vanilla replacemants with NExt. If there's no good reason for combining those I want to keep NExt separate for easier maintenance.
//
//Killface
If it's helping, then any changes are welcome.
//
Okay, I see where you are coming from. I'll just add an additional condition check for it. Here's what I have:
	
	RoadSize:Direction/#Lanes:LaneType:TransitStop:SubType:Node:TexType

	So a Small Onewy Grass Road with two bus stops will produce the following names:
		RoadSmallOnewayBusBothDeco_APRMap
		RoadSmallOnewayBusBothDeco_APRMapLOD
		RoadSmallOnewayBusBothDeco_MainTex
		RoadSmallOnewayBusBothDeco_MainTexLOD
	This way, there exists an option to change the things you want. The LODS are what neccessitated this.
	What I mean by combining both NExt and Stock roads is that the mod only checks for replacement textures that match a set criteria. This way it is easier to sort the texture files out by size.
//Killface
The naming schemes are no problem, since most of the names are unique. The LOD textures also have their unique name so there is really no need for any new scheme which differs from the vanilla names. The textures do get loaded, but never displayed. The game uses a giant, self generated map with all loaded textures - which I don't know how it's generated. This is the only problem with the lods. 

I still don't see the need of a new naming scheme. Everything gets replaced as intended.
//
	So,
	RoadSize
		RoadTiny = The tiny NExt roads
		RoadSmall = Same with the inclusion of Small Heavy Roads
		RoadMedium = Same + The NExt medium road additions [4 lane and the turning lane]
		RoadLarge = Same + 8 lane ave
		Highway=highways
	Directions/#Lanes
		Oneway
		Oneway3L
		Oneway4L
		TurningLane
		8LAveneue
		Ramp
		StateRoad
		2L
		3L
		4L
		5L
		6L
	LaneType
		BusLane
		BikeLane
		Tram
	TransitStop
		BusSide
		BusBoth
		TramStopSide
		BusStopTramStop
		TramStopBoth
	SubType
		Ground
		Deco
		Elevated
		Slope
		Tunnel
	TextType
		APRMAp
		MainTex
		LOD_APRMap
		LOD_MainTex
         
This isn't a need from a programming perspective, but it is with when producing textures. RoadSize allows for subdirs for easier sorting. I'll add the change as an aside. 

//Killface
I don't see why I should start sorting the files again when absent directories produce bugs.
Also, why sorting when it's purely cosmetical? The game doesn't care if files are in subdiretories.
Maybe you should change your setup and let Photoshop generate the pngs by composition layers and automation. No need for additional software to mass produce. PS already does that trick.
Most files titled "ground" also are used as "elevated" or "slope". This leads to double textures etc.
Regarding NExt, I like the naming scheme as it is and wont change any of my 189 files.
//


American Roads Version:
   - New Mod Name: American Infrastructure

Planned Updates
    -Replacing individual textures on segments to allow "true" turning lanes (NExt roads) w/ solid lines.

I've been keeping quiet about it, but if you look at my imgur posts, you will see my American styled traffic lights. Using his mod alongside would allow for true awesomeness and a whole new level of realism (graphics wise). //KF: they look really cool, but have no experience with assets. 

Killface:  
- Getting rid of segment.lod render distance = -1 and implement real lod textures by default
- adding optional tooltip textures for the texture packs
- multiple crossings per option; might be solved by "for each ...node_int (...node_1.dds, node_2.dds) dropdown add x - can also apply to ..._parking1

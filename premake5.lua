workspace "GAMES202"
	architecture "x64"

	configurations
	{
		"Debug",
		"Release"
	}

outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"

project "Homework0"
	location "Homework0"
	kind "ConsoleAPP"
	language "C++"

	targetdir("bin/" ..outputdir.."/%{prj.name}")
	objdir("bin-int/" ..outputdir.."/%{prj.name}")

	files
	{
		"%{prj.name}/src/**.h",
		"%{prj.name}/src/**.cpp",
		"public/**.h",
		"public/**.cpp",
		"public/**.c"
	}

	includedirs
	{
		--"%{prj.name}/vendor/spdlog/include",
		"vendor/include",
		"public"
	}

	libdirs 
	{ 
		"vendor/lib" 
	}

	links
	{	
		"assimp-vc143-mt",
		"glfw3_mt",
		"opengl32"
	}

	filter "system:windows"
		cppdialect "C++17"
		staticruntime "on"
		systemversion "10.0.19041.0"

	filter "configurations:Debug"
		defines "DEBUG"
		symbols "on"

	filter "configurations:Release"
		defines "NDEBUG"
		optimize "on"

project "Homework1"
	location "Homework1"
	kind "ConsoleAPP"
	language "C++"

	targetdir("bin/" ..outputdir.."/%{prj.name}")
	objdir("bin-int/" ..outputdir.."/%{prj.name}")

	files
	{
		"%{prj.name}/src/**.h",
		"%{prj.name}/src/**.cpp",
		"public/**.h",
		"public/**.cpp",
		"public/**.c"
	}

	includedirs
	{
		--"%{prj.name}/vendor/spdlog/include",
		"vendor/include",
		"public"
	}

	libdirs 
	{ 
		"vendor/lib" 
	}

	links
	{	
		"assimp-vc143-mt",
		"glfw3_mt",
		"opengl32"
	}

	filter "system:windows"
		cppdialect "C++17"
		staticruntime "on"
		systemversion "10.0.19041.0"

	filter "configurations:Debug"
		defines "DEBUG"
		symbols "on"

	filter "configurations:Release"
		defines "NDEBUG"
		optimize "on"


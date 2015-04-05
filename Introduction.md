# Why #

I originally created a project called XRGLib (http://code.google.com/p/xrglib/) which aimed to provide a direct .NET roguelike library, based in spirit on LibTCOD (http://doryen.eptalys.net/libtcod/).  LibTCOD is a great library, but as it is a native application, using it from .NET required interop which is often not the cleanest solution.

However, XRGLib uses XNA as its backend, which I felt to be much like using a sledgehammer to set finish nails.  Also, the interface was a bit messy, XNA tied it to Windows, and it lacked any clean way to draw tiles and sprites.

My last project required me to learn some OpenGL, and in the process I ended up writing a 2D sprite rendering framework using OpenTK.  Seeing that I now had the foundation to re-write XRGLib, I stripped and re-purposed the 2D library as a console backend, and ported everything over.

Hence, SharpRL.


# What #

What it is:
  * A C#/.NET library for getting a Roguelike game up and going quickly.
  * Handles the window, rendering, and keyboard/mouse input
  * Provides a "console" type rendering surface, to print ASCII characters in true-color
  * Supports rendering of tiles, with alpha blending, multiple tile-sheets, pixel placement, etc.
  * Uses OpenTK, a portable .NET wrapper around OpenGL.
  * Abstracts and hides OpenTK behind an abstraction layer, so no knowledge of OpenGL is needed.
  * Provides Field-of-view, line tracing, A-star pathfinding, and BSP tree algorithms and data structures.
  * Public interface methods and classes are XML documented
  * Aims to adhere to .NET and C# idioms, leveraging the BCL as necessary.
  * Aims to be optimized to allow real-time effects and large consoles
  * Theoretically directly portable to Mono (but not tested!)

What it is not:
  * A full-featured framework for RL games - you still have to program logic for most game specific items
  * Compatible with ancient computers - SharpRL requires OpenGL 2.0 or newer, and the .NET framework 4.0 or newer.
  * An all-purpose 2-D library - the tile rendering is fairly minimal, but also fairly fast


What still needs to be done:
  * Test on Windows XP/Vista (only tested under Windows7)
  * Compile for Mono and test under Linux
  * Use it for actual production...
  * Testing, testing, testing...

Things I've considered possibly adding:
  * Fleshing out and exposing more of the OpenGL backend for "special effects."  For example, perhaps allow custom shader programs for glyph and tile rendering
  * Adding a coherent noise class
  * More robust/customizable font image support - right now it is limited to 16x16 font sheets.
  * Interface for directly polling keyboard and mouse input
  * _Actually writing a roguelike game someday!_

###UPDATE 11/9/13###

Added RLGui project, this is a WIP for a text-based GUI.


![http://i.imgur.com/kpfLT.png](http://i.imgur.com/kpfLT.png)

![http://i.imgur.com/7fkOH.png](http://i.imgur.com/7fkOH.png)
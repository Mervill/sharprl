using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRL;
using SharpRL.Framework;
using System.Drawing;

namespace SharpRLTest
{

    class ExampleApp
    {
        // Store the GameConsole object, there should only ever be one of these for an application!
        GameConsole console;

        // A list of available fonts and their names
        List<FontSheet> fonts;
        List<string> fontNames;

        // A list of example objects
        List<ExampleBase> examples;

        // A variable and interpolator used to animate (fade) the font name message that appears
        // at the top of the console
        float fontNameOpacity;
        Interpolator fontInterp;

        // An array of interpolators for animating the color of each menu item
        Interpolator[] menuAnimators;

        // A general collection of interpolators, this will simplify tracking any interpolators we
        // create later
        InterpolatorCollection interpolators = new InterpolatorCollection();

        // The current active example and font
        int currentExample = 0;
        int currentFont = 0;

        // The width and height of the console applicaiton, in characters
        const int AppWidth = 100;
        const int AppHeight = 60;

        // The width and height of the example rendering area, in characters
        const int exampleWidth = 60;
        const int exampleHeight = 55;

        // For this excercise, we cheat and just provide a global Rectangle structure corresponding
        // to the example rendering area (coords and sizes are in characters)
        public static Rectangle ExampleRect;

        // These will be used to calculate the current frame rate
        int frameRate;
        int frameCount;
        float frameAccum;

        public ExampleApp()
        {
            // Create the example rect, this will be used by some of the examples
            ExampleRect = new Rectangle(AppWidth - exampleWidth - 1, 2, exampleWidth, exampleHeight);

            // Create the console, giving it a size, window title, and window area size
            console = new GameConsole(AppWidth, AppHeight, "SharpRL Examples", 800, 600);

            // We do not want to allow the user to drag-resize  the window, as this would complicate
            // matters, such as drawing the tile map
            console.IsWindowResizable = false;

            // Start off with vertical sync enabled
            console.VerticalSyncEnabled = true;

            // Now that we have a valid console, we can create the font objects
            fonts = new List<FontSheet>();
            fonts.Add(new FontSheet(console, "terminal12x12_gs_ro.png", FontFormat.GreyscaleAA, FontLayout.InRow));
            fonts.Add(new FontSheet(console, "terminal16x16_gs_ro.png", FontFormat.GreyscaleAA, FontLayout.InRow));
            fonts.Add(new FontSheet(console, "terminal8x8_aa_as.png",  FontFormat.AlphaAA, FontLayout.InColumn));
            fonts.Add(new FontSheet(console, "terminal10x16_gs_ro.png", FontFormat.GreyscaleAA, FontLayout.InRow));

            // The font name list will simply be used for displaying the current font at the top of the console
            fontNames = new List<string>()
            {
                "terminal12x12_gs_ro.png",
                "terminal16x16_gs_ro.png",
                "terminal8x8_aa_as.png",
                "terminal10x16_gs_ro.png",

            };

            // Set the current font
            console.SetFont(fonts[0]);

            // Create the examples
            examples = new List<ExampleBase>()
            {
                new Example1(),
                new Example2(),
                new Example3(),
                new Example4(),
                new Example5(),
                new Example6(console)
            };

            // Hook into the key and mouse events provided by GameConsole
            console.KeyDown += new EventHandler<EventArgs<KeyRawEventData>>(Console_KeyDown);
            console.MouseButtonDown += new EventHandler<EventArgs<MouseEventData>>(console_MouseButtonClick);

            // For animating the font name opacity, we use an interpolator coupled with a lambda to set
            // an opacity variable
            fontInterp = interpolators.AddInterpolator(3f, (i) => { fontNameOpacity = 1f - i.Progress; }, null);

            // Create an array to store an interpolator, one for each example, for animating the menu items
            menuAnimators = new Interpolator[examples.Count];

            // Note that the above are two different ways to use interpolators.  For the fonts, we explicitly give
            // the interpolator a delegate that modifies the fontNameOpacity variable.  For the menu items,
            // we keep an array of references to each corresponding interpolator, and will check each one's Progress
            // property manually (see DrawMenu and DrawFont methods below)
        }

        void console_MouseButtonClick(object sender, EventArgs<MouseEventData> e)
        {
            // If the mouse click happened inside the example area, then route
            // the event to the currently active example.
            if (ExampleRect.Contains(e.Value.CX, e.Value.CY))
            {
                // For convenience, we copy and modify the MouseEventArgs to transform
                // the position to the examples' local coordinate system
                var cx = e.Value.CX - ExampleRect.Left;
                var cy = e.Value.CY - ExampleRect.Top;

                MouseEventData mInfo = new MouseEventData(cx, cy, e.Value.PX, e.Value.PY, e.Value.Button);

                examples[currentExample].OnMouseClick(mInfo);
            }
        }

        void Console_KeyDown(object sender, EventArgs<KeyRawEventData> e)
        {
            switch (e.Value.Key)
            {
                case KeyCode.F:
                    currentFont++;
                    if (currentFont >= fonts.Count)
                        currentFont = 0;

                    // Note that by default, when changing to a font with different character size, the window
                    // will automatically scale up or down to fit the font character size.  We want this here,
                    // as it greatly simplifies positioning the tile map we will use in Example 6.
                    console.SetFont(fonts[currentFont]);

                    // Stop the font fader interpolator, in case it was still running
                    fontInterp.Stop();

                    // Create a new font fader interp
                    fontInterp = interpolators.AddInterpolator(3f, (i) => { fontNameOpacity = 1f - i.Progress; }, null);

                    // Let the current example know that the font changed.  Example 6 will use this to re-position the tile
                    // map accordingly
                    examples[currentExample].OnFontChanged();
                    break;

                case KeyCode.T:
                    // Set the GameConsole.TargetFPS accordingly.  This will be the (maximum) target frames-per-second
                    if (console.TargetFPS == 0)
                        console.TargetFPS = 30;
                    else
                        console.TargetFPS = 0;
                    break;

                case KeyCode.V:
                    // Toggle the vertical sync mode
                    console.VerticalSyncEnabled = !console.VerticalSyncEnabled;
                    break;

                case KeyCode.Down:
                    // First we stop any currently running menu animation interpolator, and create a new one.
                    if(menuAnimators[currentExample] != null)
                        menuAnimators[currentExample].Stop();
                    menuAnimators[currentExample] = interpolators.AddInterpolator(0.3f, null, null);

                    currentExample++;
                    if (currentExample >= examples.Count)
                        currentExample = 0;

                    break;

                case KeyCode.Up:
                    if (menuAnimators[currentExample] != null)
                        menuAnimators[currentExample].Stop();
                    menuAnimators[currentExample] = interpolators.AddInterpolator(0.3f, null, null);

                    currentExample--;
                    if (currentExample < 0)
                        currentExample = examples.Count - 1;
                    break;

                case KeyCode.Escape:
                    console.Exit();
                    break;
            }

            // Pass on the key event to the currently active example
            examples[currentExample].OnKey(e.Value);
        }

        // This is our main entry into the game loop, called from our Main method
        public void Run()
        {
            // We provde the OnTick delegate that will get called each render frame
            console.Run(OnTick);

            // Make sure to dispose of the renderer when we are finished with it
            console.Dispose();
        }

        void OnTick(float elapsed)
        {
            // Calculate average measured FPS
            frameCount++;
            frameAccum += elapsed;

            if (frameAccum >= 1.0f)
            {
                frameRate = frameCount;

                frameAccum = 0;
                frameCount = 0;
            }

            // Update interpolators.  The InterpolatorCollection automatically handles updating
            // all of its contained interps, as well as automatically removing inactive ones from the
            // collection in a safe manner, so we don't have to worry about it.
            interpolators.Update(elapsed);

            // Update the current example.  Note that we have opted to split the render loop into
            // an update and draw phase in our examples.  This is optional, but in our case ExampleBase.Draw
            // is special in that it returns a reference to it's own surface, so having a separate Update
            // probably works cleaner here.
            examples[currentExample].Update(elapsed);

            // Clear the root surface
            console.Root.Clear();

            // Call the current example's Draw method, which returns a reference to it's surface
            Surface surface = examples[currentExample].Draw();

            // Blit the example's offscreen surface to the root surface in the example rendering area
            Surface.Blit(surface, console.Root, ExampleRect.Left, ExampleRect.Top);

            // Draw a frame around the example rendering area
            console.Root.DrawFrame(Rectangle.Inflate(ExampleRect, 1, 1));

            // Now draw the status, menu and font name
            DrawStatus();
            DrawMenu();
            DrawFontName();
        }



        // Here we draw the status area at the bottom of the window
        void DrawStatus()
        {
            string status = string.Format("FPS: {0} {1} - {2}",
                frameRate, console.TargetFPS != 0 ? "(Fixed Time Step" + console.TargetFPS + "fps)" : "(Unfixed Time Step)",
                console.VerticalSyncEnabled ? "VSync on" : "VSync off");

            // We print the status and the command list near the bottom of the console and centered left to right
            console.Root.PrintStringAligned(0, AppHeight - 2, status, AppWidth, HorizontalAlignment.Center);

            console.Root.PrintStringAligned(0, AppHeight - 1, "[F] to change font, [T] to toggle fixed time step, [V] to toggle V-sync",
                AppWidth, HorizontalAlignment.Center);
        }


        // Now draw the menu in the upper left area of the window
        void DrawMenu()
        {
            // Loop through and print each example's Description property
            for (int i = 0; i < examples.Count; i++)
            {
                string str = examples[i].Description;

                // For the currently active example, highlight it
                if (i == currentExample)
                {
                    console.Root.PrintString(0, i, str, Color.Black, Color.Gray);
                }
                else
                {
                    // If there is a currently active interpolator for this menu item, we retreive
                    // its progress for use as a color animation factor

                    float val = 1f;
                    if (menuAnimators[i] != null && menuAnimators[i].IsActive)
                    {
                        val = menuAnimators[i].Progress;
                    }

                    // Examples of using the ColorHelper.Lerp method
                    // The forground will be white if the progres is 0, CadetBlue if 1, or somehwere in between
                    Color menuFGColor = ColorHelper.Lerp(Color.White, Color.CadetBlue, val);

                    // For the background, White if 0, Black if 1
                    Color menuBGColor = ColorHelper.Lerp(Color.White, Color.Black, val);

                    console.Root.PrintString(0, i, str, menuFGColor, menuBGColor);
                }
            }
        }

        void DrawFontName()
        {
            // If the font opacity is greater than 0, we print the font name
            if (fontNameOpacity > 0f)
            {
                // Fade the font name according to fontNameOpacity, which is being controlled by an interpolator.
                // We could do the following:
                //Color labelColor = ColorHelper.Lerp(Color.Black, Color.White, fontNameOpacity);
                // But here is a demonstration of getting the same effect by using ColorHelper.CreateFromValue() method,
                // which is one of several helper methods that manipulate colors in hue/saturation/value color space

                Color labelColor = ColorHelper.CreateFromValue(Color.White, fontNameOpacity);

                // Then print it at the top center of the console using the calculated color as the foreground color
                console.Root.PrintStringAligned(0, 0, fontNames[currentFont], console.Root.Width,
                    HorizontalAlignment.Center, labelColor);
            }
        }
    }
}

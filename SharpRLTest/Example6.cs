using System;
using System.Collections.Generic;
using SharpRL;
using SharpRL.Toolkit;
using System.Drawing;

namespace SharpRLTest
{
    class Example6 : ExampleBase
    {
        GameConsole console;
        TileSheet tiles;
        Point playerLocation;
        Point newPlayerLocation;

        public Example6(GameConsole console)
        {
            this.console = console;

            tiles = new TileSheet(console, "tiles.png", 32, 32);

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (!map[x, y].IsWall)
                    {
                        playerLocation = new Point(x, y);
                        break;
                    }
                }
                if (playerLocation != Point.Empty)
                    break;
            }

        }

        public override Surface Draw()
        {
            DrawTileMap();

            return base.Draw();
        }

        public override void OnKey(KeyRawEventData key)
        {
            base.OnKey(key);

            newPlayerLocation = playerLocation;

            switch (key.Key)
            {
                case KeyCode.W:
                    newPlayerLocation.Y--;
                    break;

                case KeyCode.D:
                    newPlayerLocation.X++;
                    break;

                case KeyCode.S:
                    newPlayerLocation.Y++;
                    break;

                case KeyCode.A:
                    newPlayerLocation.X--;
                    break;
            }
        }

        bool CheckMapValidLocation(int cx, int cy)
        {
            if (cx < 0 || cy < 0)
                return false;

            if (!map[cx, cy].IsWalkable(null))
                return false;

            return true;
        }

        void DrawTileMap()
        {
            // Create the tile map viewport in the example rendering area.  Here we just do it each time
            // we draw, a cleaner approach would be to wrap all this into a tile map object

            // first get the font character size
            int fontWidth = console.CurrentFont.CharacterWidth;
            int fontHeight = console.CurrentFont.CharacterHeight;

            // Pre-calc the rendering area in pixel coordinates
            Rectangle exampleRectPixels = new Rectangle(ExampleApp.ExampleRect.X * fontWidth,
                ExampleApp.ExampleRect.Y * fontHeight,
                ExampleApp.ExampleRect.Width * fontWidth,
                ExampleApp.ExampleRect.Height * fontHeight);

            // Calc the number of cells in the tile map that will fit in the example rendering area
            int tmWidthIncells = exampleRectPixels.Width / tiles.TileWidth;
            int tmHeightInCells = exampleRectPixels.Height / tiles.TileHeight;

            // Get the remainder of the last operation, to be used to center the tile map
            int tmWidthRemain = exampleRectPixels.Width % tiles.TileWidth;
            int tmHeightRemain = exampleRectPixels.Height % tiles.TileHeight;

            // Final position of the tile map, centered (approx.) in the example area
            int tmPosX = exampleRectPixels.X + tmWidthRemain / 2;
            int tmPosY = exampleRectPixels.Y + tmHeightRemain / 2;

            Point dest = Point.Empty;
            Point src = Point.Empty;

            // Now draw the tile map
            for (int y = 0; y < tmHeightInCells; y++)
            {
                for (int x = 0; x < tmWidthIncells; x++)
                {
                    dest.X = tmPosX + x * tiles.TileWidth;
                    dest.Y = tmPosY + y * tiles.TileHeight;

                    if (map[x, y].IsWall)
                    {
                        src.X = 1;
                        src.Y = 0;
                    }
                    else
                    {
                        src.X = 0;
                        src.Y = 0;
                    }

                    console.DrawTile(tiles, dest, src, Color.White);
                }
            }

            // Check if the next player position is valid
            if (newPlayerLocation.X > 0 && newPlayerLocation.X < tmWidthIncells &&
                newPlayerLocation.Y > 0 && newPlayerLocation.Y < tmHeightInCells &&
                map[newPlayerLocation.X, newPlayerLocation.Y].IsWalkable(null))
            {
                playerLocation = newPlayerLocation;
            }

            // Draw the player tile
            dest.X = playerLocation.X * tiles.TileWidth + tmPosX;
            dest.Y = playerLocation.Y * tiles.TileHeight + tmPosY;

            console.DrawTile(tiles, dest, new Point(0, 1), Color.White);
        }

        public override string Description
        {
            get { return "Tile Map"; }
        }

    }
}

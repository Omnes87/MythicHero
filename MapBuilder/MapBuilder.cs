namespace MapBuilder
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    public class MapBuilder : Form
    {
        private const int TileSize = 20;

        private const int TilesInGridRow = 32;

        private const int TilesInGridColumn = 24;

        private const int TilesInToolbarRow = 6;

        private const int TilesInToolbarColumn = 24;

        private const int GridWidth = (TileSize + 1) * TilesInGridRow + 1;

        private const int GridHeight = (TileSize + 1) * TilesInGridColumn + 1;

        private const int ToolbarWidth = (TileSize + 1) * TilesInToolbarRow;

        private const int ToolbarHeight = (TileSize + 1) * TilesInToolbarColumn;

        private const int SplitterWidth = 12;

        private const int FileTreeHeight = 200;

        private readonly string MapRoot = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\MythicHero\MythicHero\Field\Data"));

        private readonly string TileRoot = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\MythicHero\MythicHeroContent\Image\Field"));

        private readonly string[,] tiles = new string[TilesInGridColumn, TilesInGridRow];

        private readonly PictureBox[,] tilePictures = new PictureBox[TilesInGridColumn, TilesInGridRow];

        private readonly Dictionary<string, Image> assetCache = new Dictionary<string,Image>(StringComparer.OrdinalIgnoreCase);

        private readonly PictureBox currentTileLeft;

        private readonly PictureBox currentTileRight;

        private string filename;

        private bool isDirty = true;

        public MapBuilder()
        {
            this.SuspendLayout();

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(GridWidth + SplitterWidth + ToolbarWidth, GridHeight);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Name = "MapBuilder";
            this.Text = "Map Builder";

            var splitContainer = new SplitContainer();
            splitContainer.SuspendLayout();

            splitContainer.Dock = DockStyle.Fill;
            splitContainer.ForeColor = SystemColors.Control;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Vertical;

            splitContainer.Size = this.ClientSize;
            splitContainer.SplitterDistance = GridWidth;
            splitContainer.SplitterWidth = 12;
            splitContainer.Text = "splitContainer";
            splitContainer.IsSplitterFixed = true;

            splitContainer.Panel1.BackColor = SystemColors.Control;
            splitContainer.Panel1.Name = "splitterPanel1";

            splitContainer.Panel2.BackColor = SystemColors.Control;
            splitContainer.Panel2.Name = "splitterPanel2";

            var toolbarContainer = new SplitContainer();
            toolbarContainer.SuspendLayout();

            toolbarContainer.Dock = DockStyle.Fill;
            toolbarContainer.ForeColor = SystemColors.Control;
            toolbarContainer.Location = new Point(0, 0);
            toolbarContainer.Name = "toolbarContainer";
            toolbarContainer.Orientation = Orientation.Horizontal;

            toolbarContainer.Size = new Size(ToolbarWidth, ToolbarHeight);
            toolbarContainer.SplitterDistance = FileTreeHeight;
            toolbarContainer.SplitterWidth = 12;
            toolbarContainer.Text = "toolbarContainer";
            toolbarContainer.IsSplitterFixed = true;

            toolbarContainer.Panel1.BackColor = SystemColors.Control;
            toolbarContainer.Panel1.Name = "toolbarPanel1";

            var fileTree = new TreeView();
            fileTree.Dock = DockStyle.Fill;
            fileTree.BeginUpdate();
            BuildFileTree(fileTree.Nodes, TileRoot);
            fileTree.ExpandAll();
            fileTree.EndUpdate();

            var fileTreeGroup = new GroupBox();
            fileTreeGroup.Size = new Size(ToolbarWidth, FileTreeHeight);
            fileTreeGroup.Text = "Assets";
            fileTreeGroup.Controls.Add(fileTree);

            toolbarContainer.Panel1.Controls.Add(fileTreeGroup);

            toolbarContainer.Panel2.BackColor = SystemColors.Control;
            toolbarContainer.Panel2.Name = "toolbarPanel2";

            this.currentTileLeft = new PictureBox();
            this.currentTileLeft.Location = new Point((int)(splitContainer.Panel2.Size.Width / 2 - 1.5 * TileSize), 25);
            this.currentTileLeft.Size = new Size(TileSize, TileSize);
            this.currentTileLeft.BorderStyle = BorderStyle.FixedSingle;

            this.currentTileRight = new PictureBox();
            this.currentTileRight.Location = new Point((int)(splitContainer.Panel2.Size.Width / 2 + 0.5 * TileSize), 25);
            this.currentTileRight.Size = new Size(TileSize, TileSize);
            this.currentTileRight.BorderStyle = BorderStyle.FixedSingle;

            var currentTiles = new GroupBox();
            currentTiles.Text = "Current Tiles";
            currentTiles.Size = new Size(splitContainer.Panel2.Size.Width, 60);
            currentTiles.Controls.Add(currentTileLeft);
            currentTiles.Controls.Add(currentTileRight);

            toolbarContainer.Panel2.Controls.Add(currentTiles);
            splitContainer.Panel2.Controls.Add(toolbarContainer);

            BuildMenu();

            for (int x = 0; x < TilesInGridColumn; x++)
            {
                for (int y = 0; y < TilesInGridRow; y++)
                {
                    var pictureBox = new PictureBox();
                    pictureBox.Location = new Point((TileSize + 1) * y + 1, (TileSize + 1) * x + 1);
                    pictureBox.Size = new Size(TileSize, TileSize);
                    this.Controls.Add(pictureBox);
                    tilePictures[x, y] = pictureBox;
                }
            }

            this.Controls.Add(splitContainer);

            splitContainer.Panel1.Paint += this.UpdateTitle;
            splitContainer.Panel1.Paint += this.DrawGrid;
            splitContainer.Panel2.Paint += this.DrawToolbar;

            toolbarContainer.ResumeLayout(false);
            splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void BuildMenu()
        {
            var fileNewItem = new MenuItem("&New");
            fileNewItem.Click += this.FileNewClicked;

            var fileOpenItem = new MenuItem("&Open");
            fileOpenItem.Click += this.FileOpenClicked;

            var fileSaveItem = new MenuItem("&Save");
            fileSaveItem.Click += this.FileSaveClicked;

            var fileSaveAsItem = new MenuItem("Save &As");
            fileSaveAsItem.Click += this.FileSaveAsClicked;

            var fileMenu = new MenuItem("&File");
            fileMenu.MenuItems.Add(fileNewItem);
            fileMenu.MenuItems.Add(fileOpenItem);
            fileMenu.MenuItems.Add(fileSaveItem);
            fileMenu.MenuItems.Add(fileSaveAsItem);

            var menu = new MainMenu();
            menu.MenuItems.Add(fileMenu);

            this.Menu = menu;
        }

        private void BuildFileTree(TreeNodeCollection nodes, string directory)
        {
            var node = nodes.Add(directory, Path.GetFileName(directory));
            foreach(var subDirectory in Directory.EnumerateDirectories(directory))
            {
                BuildFileTree(node.Nodes, subDirectory);
            }
        }

        private void FileNewClicked(object sender, EventArgs e)
        {
            // Null out all tiles
            for (int x = 0; x < TilesInGridColumn; x++)
            {
                for (int y = 0; y < TilesInGridRow; y++)
                {
                    tiles[x, y] = null;
                }
            }

            this.filename = null;
            this.isDirty = true;

            this.Refresh();
        }

        private void FileOpenClicked(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.InitialDirectory = MapRoot;
            dialog.Filter = "xml files (*.xml)|*.xml";
            dialog.FilterIndex = 0;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Null out all tiles
                for (int x = 0; x < TilesInGridColumn; x++)
                {
                    for (int y = 0; y < TilesInGridRow; y++)
                    {
                        tiles[x, y] = null;
                    }
                }

                this.isDirty = false;

                try
                {
                    this.filename = dialog.FileName;
                    using (var xmlReader = XmlReader.Create(dialog.OpenFile()))
                    {
                        xmlReader.ReadStartElement("Map");

                        for (int x = 0; x < TilesInGridColumn; x++)
                        {
                            xmlReader.ReadStartElement("Row");
                            for (int y = 0; y < TilesInGridRow; y++)
                            {
                                xmlReader.ReadStartElement("Tile");

                                var textureName = xmlReader.ReadString();
                                tiles[x, y] = textureName;

                                xmlReader.ReadEndElement();
                            }

                            xmlReader.ReadEndElement();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. " + ex.Message);
                }
            }

            this.Refresh();
        }

        private void FileSaveClicked(object sender, EventArgs e)
        {
            // TODO: Implement
        }

        private void FileSaveAsClicked(object sender, EventArgs e)
        {
            // TODO: Implement
        }

        private void UpdateTitle(object sender, PaintEventArgs e)
        {
            var title = "Map Builder - ";
            title += this.filename == null ? "untitled map" : Path.GetFileNameWithoutExtension(this.filename);
            if (this.isDirty)
            {
                title += "*";
            }

            this.Text = title;
        }

        private void DrawGrid(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;

            for (int x = 0; x < TilesInGridColumn; x++)
            {
                for (int y = 0; y < TilesInGridRow; y++)
                {
                    var assetName = tiles[x, y];
                    if (assetName == null)
                    {
                        this.tilePictures[x, y].Image = null;
                    }
                    else
                    {
                        this.tilePictures[x, y].Image = this.GetAsset(assetName);
                    }
                }
            }

            // Draw horizontal lines
            for (int i = 0; i < TilesInGridColumn + 1; i++)
            {
                var y = i * (TileSize + 1);
                graphics.DrawLine(Pens.Black, new Point(0, y), new Point(GridWidth, y));
            }

            // Draw vertical lines
            for (int i = 0; i < TilesInGridRow + 1; i++)
            {
                var x = i * (TileSize + 1);
                graphics.DrawLine(Pens.Black, new Point(x, 0), new Point(x, GridHeight));
            }
        }

        private void DrawToolbar(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
        }

        private Image GetAsset(string assetName)
        {
            Image image;
            if (!this.assetCache.TryGetValue(assetName, out image))
            {
                image = Image.FromFile(Path.Combine(TileRoot, assetName + ".jpg"));
                this.assetCache.Add(assetName, image);
            }

            return image;
        }
    }
}

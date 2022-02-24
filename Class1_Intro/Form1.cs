using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms ;
using System.Collections.Generic;


namespace Class1_Intro
{ 
    // The Universe
    public partial class Form1 : Form
    {         
        // universe[]
        bool[,] universe = new bool[30, 20]; 
        bool[,] Pad = new bool[30, 20];
        bool[,] temp = new bool[30, 20];

        // colors
        Color gridColor = Color.White;
        Color cellColor = Color.White;
        Color deadcellC = Color.Black;

        // timer
        Timer timer = new Timer(); 
   
        // gen count
        int generations = 0;
        int aliveCells = 0; 

        // forms
        GetSeedForRand dudex = new GetSeedForRand();
        Settings option = new Settings(); 

        // initial 
        public bool[,] Universe { get => universe; set => universe = value; }

        #region Form1
        public Form1()
        {        
            InitializeComponent();

            option.OneColor = gridColor;
            option.SecondColor = deadcellC;
            option.OneMoreColor = cellColor;

            // inital timer 
            timer.Tick += Timer_Tick;
            timer.Interval = 50;
            option.interval = timer.Interval;
            CellsAlive.Text = "Living Cells: " + aliveCells.ToString();
        }
        #endregion

        #region Timer
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            countAliveCells();
        }
        #endregion

        #region Counting Neighbors
        private int countNeighbor( int a, int b)
        {
            int ans = 0;

            for (int i = -1; i < 2; i++)
            {
                // iterate through the universe left to right (x axis)
                for (int j = -1; j < 2; j++)
                {
                    int col = (b + i + universe.GetLength(1)) % universe.GetLength(1);

                    int row = (a + j + universe.GetLength(0)) % universe.GetLength(0);

                    if (universe[row, col] == true)
                    {
                        ans++;
                    }
                }

            }

            if (universe[a, b])
            {
                ans--;
            }

            return ans;
        }
        #endregion

        #region Next Generation
        private void NextGeneration()
        {
            temp = universe;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    var state = universe[x, y];

                    int neighbors = countNeighbor(x, y);

                    if (state == false && neighbors == 3)
                    {
                        Pad[x, y] = true;
                    }
                    else if (state == true && (neighbors < 2 || neighbors > 3))
                    {
                        Pad[x, y] = false;
                    }
                    else
                    {
                        Pad[x, y] = state;
                    }
                }
            }

            universe = Pad;
            Pad = temp;
            generations++;
            toolStripStatusLabelGerenations.Text = "Generations = " + generations.ToString();

            graphicsPanel1.Invalidate();
        }
        #endregion
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        #region Graphic Tools
        // graphic tools
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // floats!?
            Font font = new Font("Arial", 10f);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
           
            // The width and height of each cell in pixels
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);  

            // Brush
            Brush cellBrush = new SolidBrush(cellColor);
            Brush DeadCellBrush = new SolidBrush(deadcellC);

            // iterate through the universe top to bottom (y axis)
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // iterate through the universe left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    // RectangleF for floats!
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else if (universe[x,y] == false)
                    {
                        e.Graphics.FillRectangle(DeadCellBrush, cellRect);
                    } 

                    e.Graphics.DrawString(countNeighbor(x, y).ToString(), font, Brushes.Black, cellRect, stringFormat);

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            gridPen.Dispose();
            cellBrush.Dispose();           
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // floats!?
                // The width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                int i = e.X / cellWidth;
                int j = e.Y / cellHeight;

                universe[i, j] = !universe[i, j];
                countAliveCells();

                graphicsPanel1.Invalidate();
            }
        }
        #endregion

        #region Universe Reset
        // this function is to Reset the Universe
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // In case the Timer is On 
            timer.Enabled = false;

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }

            // generation ands alive cells set to 0
            generations = 0;
            aliveCells = 0;
            toolStripStatusLabelGerenations.Text = "Generations = " + generations.ToString();
            CellsAlive.Text = "Living Cells: " + aliveCells.ToString();
            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Play Button
        private void PlayB_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }
        #endregion

        #region Stop Button
        private void Stop_Timer_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        #endregion

        #region Random function
        private void basicRandomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int state;
            Random dude = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {

                    state = dude.Next(0, 2);

                    if (state == 0)
                    {
                        universe[x, y] = false;
                    }
                    else
                    {
                        universe[x, y] = true;
                    }
                }
            }
            countAliveCells();
            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Random Dialog
        private void seedRandomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(dudex.ShowDialog() == DialogResult.OK)

            {
                int state;
                Random dude = new Random(dudex.myVar); 

                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {

                        state = dude.Next(0, 2);

                        if (state == 0)
                        {
                            universe[x, y] = false;
                        }
                        else
                        {
                            universe[x, y] = true;
                        }
                    }
                }

                countAliveCells();
                graphicsPanel1.Invalidate();

            }
            else 
            {
               // dudex.Close();
            }

        }
        #endregion

        #region Saving the file
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;         

            SaveFileDialog saveFileDialog1 = new SaveFileDialog(); 
            saveFileDialog1.DefaultExt = "cells";
            saveFileDialog1.Title = "Saving the universe.";          
            saveFileDialog1.Filter = "Cells and texts files (*.txt),(*.cells)|*.txt,*.cells|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // saves the image via a FileStream created by the OpenFile method 
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);

                sw.WriteLine("!Name: " + Path.GetFileNameWithoutExtension(saveFileDialog1.FileName));
                sw.WriteLine("!");

                for ( y = 0; y < universe.GetLength(1); y++)
                    {

                    if (y > 0)
                    {
                        sw.WriteLine();
                    }                 

                    for ( x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == false)
                        {
                            sw.Write(".");
                        }

                        else
                        {
                            sw.Write("O");
                        }
                    }
                    }

                // Saves the Image in the appropriate ImageFormat based upon the  
                // File type selected in the dialog box.  
                // NOTE that the FilterIndex property is one-based.   

                sw.Close();                   
            }

        }
        #endregion

        #region Counting the living cells
        void countAliveCells()
        {
            aliveCells = 0; 

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {

                    if (universe[x,y] == true)
                    {
                        aliveCells++;
                    } 
                }
            }

            CellsAlive.Text = "Living Cells: " + aliveCells.ToString();
        }
        #endregion

        #region Open a file and read and import new Universe
        private void openToolStripButton_Click(object sender, EventArgs e)
        { 
            string line = "";
            int counterY = 0;
            int lineX = 0;

            // saving the universe
            OpenFileDialog openfile = new OpenFileDialog();

            openfile.DefaultExt = "cells";
            openfile.Title = "Entering the Universe.";
            openfile.Filter = "cells files (*.cells)|*.cells|Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            openfile.CheckPathExists = true;

            if (openfile.ShowDialog() == DialogResult.OK)
            {
                // saves the image via a FileStream created by the OpenFile method.   
                Stream s = new MemoryStream();
                StreamReader sr = new StreamReader(openfile.FileName); 
                              
                while ((line = sr.ReadLine()) != null)
                {
                    if (line[0] == '!')
                    {
                        continue;
                    }
                    else if (line[1] == '!')
                    {
                        continue;
                    }
                    lineX = line.Length;
                    counterY++;
                }

                sr.Close();

                // resize the universe 
                universe = new bool[lineX, counterY];
                Pad = new bool[lineX, counterY];

                lineX--;
                counterY = 0;

                StreamReader st = new StreamReader(openfile.FileName);

                while ((line = st.ReadLine()) != null)
                {
                    if (line[0] == '!')
                    {
                        continue;
                    }
                            
                    for (int x = 0; x < lineX; x++)
                    {

                        if (line[x] == '.')
                        {
                            universe[x, counterY] = false;
                        }

                        else if (line[x] == 'O')
                        {
                            universe[x, counterY] = true;
                        }

                    }
                    counterY++;                               
                }
                st.Close();
            }
            countAliveCells();
            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Option Menu
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (option.ShowDialog() == DialogResult.OK)
            {         
                // updating colors
                gridColor = option.OneColor;
                deadcellC = option.SecondColor;
                cellColor = option.OneMoreColor;

                // updating interval
                timer.Interval = option.interval;

                // updating universe size
                universe = new bool[option.X, option.Y];
                Pad = new bool[option.X, option.Y];
                temp = new bool[option.X, option.Y];
            }
            graphicsPanel1.Invalidate();
        }
        #endregion

    }
}

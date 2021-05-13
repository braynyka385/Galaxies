using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Galaxies
{
    public partial class Form1 : Form
    {
        float camScale = 2f;
        bool[] pressedKeys = new bool[4];
        int camX = -1000;
        int camY = -1000;
        int initOffs = 1000;
        double G = 0.01; //0.0000000000667
        List<Galaxy> galaxies = new List<Galaxy>();   
        Pen starPen = new Pen(Color.White, 1);
        Pen redPen = new Pen(Color.OrangeRed, 1);
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.WindowsDefaultLocation;
            //Star count, total mass, locX, locY, has a core, are stars moving off start, the min (0 - 0.99; use higher for further distance from 0, 0), the density of the stars//


            Galaxy g = new Galaxy(800, 800, 500 + initOffs, 1200 + initOffs, false, false, 0.20f, 4f); //200
            Galaxy g2 = new Galaxy(400, 400, 1000 + initOffs, 1200 + initOffs, false, false, 0.20f, 2f);
            Galaxy g3 = new Galaxy(200, 200, 1500 + initOffs, 1600 + initOffs, false, false, 0.20f, 2f);
            Galaxy g4 = new Galaxy(100, 100, 3000 + initOffs, 1400 + initOffs, false, false, 0.20f, 2f);

            galaxies.Add(g);
            galaxies.Add(g2);
            galaxies.Add(g3);
            galaxies.Add(g4);

        }
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            KeyPresses();
            Task.Run(() => InternalGalaxyMechanisms());
            //InternalGalaxyMechanisms();
            if(galaxies.Count > 1)
            {
                Task.Run(() => ExternalGalaxyMechanisms());
                //ExternalGalaxyMechanisms();
            }
            Refresh();
        }
        public void KeyPresses()
        {
            int moveSensitivity = 5;
            if (pressedKeys[0])
            {
                camY += moveSensitivity;
            }
            if (pressedKeys[1])
            {
                camY -= moveSensitivity;
            }
            if (pressedKeys[2])
            {
                camX += moveSensitivity;
            }
            if (pressedKeys[3])
            {
                camX -= moveSensitivity;
            }
        }
        public async Task InternalGalaxyMechanisms()
        {
            foreach (Galaxy g in galaxies)
            {
                for (int i = 0; i < g.stars.Count; i++)
                {
                    g.stars[i].force[0] = 0;
                    g.stars[i].force[1] = 0;

                    g.stars[i].accel[0] = 0;
                    g.stars[i].accel[1] = 0;
                }
                for (int i = 0; i < g.stars.Count; i++)
                {
                    for (int j = i + 1; j < g.stars.Count; j++)
                    {
                        g.stars[i].interactedMass = g.stars[j].mass;
                        g.stars[j].interactedMass = g.stars[i].mass;
                        double xDist = Math.Abs(g.stars[i].x - g.stars[j].x);
                        double yDist = Math.Abs(g.stars[i].y - g.stars[j].y);
                        double dist = Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));
                        if (dist == 0)
                        {
                            dist = Math.Sqrt(Math.Pow(xDist, 2) + 1 + Math.Pow(yDist, 2) + 1);
                        }
                        double xForce;
                        if (dist == 0)
                        {
                            xForce = (G * g.stars[i].mass * g.stars[j].mass) / dist + 1;
                        }
                        else
                        {
                            xForce = (G * g.stars[i].mass * g.stars[j].mass) / dist;
                        }
                        double yForce;

                        if (dist == 0)
                        {
                            yForce = (G * g.stars[i].mass * g.stars[j].mass) / dist + 1;
                        }
                        else
                        {
                            yForce = (G * g.stars[i].mass * g.stars[j].mass) / dist;
                        }

                        double dirX = Math.Abs(g.stars[j].x - g.stars[i].x);
                        double dirY = Math.Abs(g.stars[j].y - g.stars[i].y);

                        double percentX = dirX / (dirX + dirY) * 1;
                        double percentY = 1 - percentX;
                        if (dirX == 0)
                        {
                            percentX = 0;
                            percentY = 1;
                        }
                        else if (dirY == 0)
                        {
                            percentX = 1;
                            percentY = 0;
                        }
                        if (g.stars[j].x - g.stars[i].x < 0)
                        {
                            percentX = -percentX;
                        }
                        if (g.stars[j].y - g.stars[i].y < 0)
                        {
                            percentY = -percentY;
                        }
                        g.stars[i].force[0] += xForce * percentX;
                        g.stars[i].force[1] += yForce * percentY;

                        g.stars[j].force[0] -= xForce * percentX;
                        g.stars[j].force[1] -= yForce * percentY;



                        if (g.stars[i].force[0] != g.stars[i].force[0])
                        {
                            int xad = 0; //Debugging for NaN
                        }
                    }
                    g.stars[i].accel[0] += g.stars[i].force[0] / g.stars[i].mass;
                    g.stars[i].accel[1] += g.stars[i].force[1] / g.stars[i].mass;
                    int accelCap = 500000;
                    if (g.stars[i].accel[0] > accelCap)
                    {
                        g.stars[i].accel[0] = accelCap;
                    }
                    if (g.stars[i].accel[0] < -accelCap)
                    {
                        g.stars[i].accel[0] = -accelCap;
                    }
                    if (g.stars[i].accel[1] > accelCap)
                    {
                        g.stars[i].accel[1] = accelCap;
                    }
                    if (g.stars[i].accel[1] < -accelCap)
                    {
                        g.stars[i].accel[1] = -accelCap;
                    }
                }
                foreach (Star s in g.stars)
                {
                    s.speed[0] += s.accel[0]; // / (20 / gameTimer.Interval);
                    s.speed[1] += s.accel[1]; // / (20 / gameTimer.Interval);
                    s.x += s.speed[0];
                    s.y += s.speed[1];
                    double speedCap = 110000;
                    if (s.speed[0] > speedCap)
                    {
                        s.speed[0] = speedCap;
                    }
                    if (s.speed[0] < -speedCap)
                    {
                        s.speed[0] = -speedCap;
                    }
                    if (s.speed[1] > speedCap)
                    {
                        s.speed[1] = speedCap;
                    }
                    if (s.speed[1] < -speedCap)
                    {
                        s.speed[1] = -speedCap;
                    }
                }
            }
        }

        public async Task ExternalGalaxyMechanisms()
        {
            for (int g = 0; g < galaxies.Count - 1; g++)
            {
                for (int x = g + 1; x < galaxies.Count; x++)
                {
                    for (int i = 0; i < galaxies[g].stars.Count; i++)
                    {
                        //galaxies[g].stars[i].force[0] = 0;
                        //galaxies[g].stars[i].force[1] = 0;

                        for (int j = 0; j < galaxies[x].stars.Count; j++)
                        {
                            //galaxies[x].stars[j].force[0] = 0;
                            //galaxies[x].stars[j].force[1] = 0;


                            double xDist = Math.Abs(galaxies[g].stars[i].x - galaxies[x].stars[j].x);
                            double yDist = Math.Abs(galaxies[g].stars[i].y - galaxies[x].stars[j].y);
                            double dist = Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));
                            if (dist == 0)
                            {
                                dist = Math.Sqrt(Math.Pow(xDist, 2) + 1 + Math.Pow(yDist, 2) + 1);
                            }
                            double xForce;
                            if (dist == 0)
                            {
                                xForce = (G * galaxies[g].stars[i].mass * galaxies[x].stars[j].mass) / dist + 1;
                            }
                            else
                            {
                                xForce = (G * galaxies[g].stars[i].mass * galaxies[x].stars[j].mass) / dist;
                            }
                            double yForce;

                            if (dist == 0)
                            {
                                yForce = (G * galaxies[g].stars[i].mass * galaxies[x].stars[j].mass) / dist + 1;
                            }
                            else
                            {
                                yForce = (G * galaxies[g].stars[i].mass * galaxies[x].stars[j].mass) / dist;
                            }

                            double dirX = Math.Abs(galaxies[x].stars[j].x - galaxies[g].stars[i].x);
                            double dirY = Math.Abs(galaxies[x].stars[j].y - galaxies[g].stars[i].y);

                            double percentX = dirX / (dirX + dirY) * 1;
                            double percentY = 1 - percentX;
                            if (dirX == 0)
                            {
                                percentX = 0;
                                percentY = 1;
                            }
                            else if (dirY == 0)
                            {
                                percentX = 1;
                                percentY = 0;
                            }
                            if (galaxies[x].stars[j].x - galaxies[g].stars[i].x < 0)
                            {
                                percentX = -percentX;
                            }
                            if (galaxies[x].stars[j].y - galaxies[g].stars[i].y < 0)
                            {
                                percentY = -percentY;
                            }
                            galaxies[g].stars[i].force[0] += xForce * percentX;
                            galaxies[g].stars[i].force[1] += yForce * percentY;

                            galaxies[x].stars[j].force[0] -= xForce * percentX;
                            galaxies[x].stars[j].force[1] -= yForce * percentY;



                            if (galaxies[g].stars[i].force[0] != galaxies[g].stars[i].force[0])
                            {
                                int xad = 0; //Debugging for NaN
                            }
                            
                        }
                        
                    }
                }
                
            }

            ForceToAccel();
            AccelToSpeed();
            
        }

        public void AccelToSpeed()
        {
            foreach (Galaxy g in galaxies)
            {
                for (int i = 0; i < g.stars.Count; i++)
                {
                    g.stars[i].speed[0] += g.stars[i].accel[0]; // / (20 / gameTimer.Interval);
                    g.stars[i].speed[1] += g.stars[i].accel[1]; // / (20 / gameTimer.Interval);
                    g.stars[i].x += g.stars[i].speed[0];
                    g.stars[i].y += g.stars[i].speed[1];
                    double speedCap = 110000;
                    if (g.stars[i].speed[0] > speedCap)
                    {
                        g.stars[i].speed[0] = speedCap;
                    }
                    if (g.stars[i].speed[0] < -speedCap)
                    {
                        g.stars[i].speed[0] = -speedCap;
                    }
                    if (g.stars[i].speed[1] > speedCap)
                    {
                        g.stars[i].speed[1] = speedCap;
                    }
                    if (g.stars[i].speed[1] < -speedCap)
                    {
                        g.stars[i].speed[1] = -speedCap;
                    }
                    g.stars[i].accel[0] = 0;
                    g.stars[i].accel[1] = 0;
                }
            }
            
        }

        public void ForceToAccel()
        {
            foreach (Galaxy g in galaxies)
            {
                for (int i = 0; i < g.stars.Count; i++)
                {
                    g.stars[i].accel[0] += g.stars[i].force[0] / g.stars[i].mass;
                    g.stars[i].accel[1] += g.stars[i].force[1] / g.stars[i].mass;
                    int accelCap = 500000;
                    if (g.stars[i].accel[0] > accelCap)
                    {
                        g.stars[i].accel[0] = accelCap;
                    }
                    if (g.stars[i].accel[0] < -accelCap)
                    {
                        g.stars[i].accel[0] = -accelCap;
                    }
                    if (g.stars[i].accel[1] > accelCap)
                    {
                        g.stars[i].accel[1] = accelCap;
                    }
                    if (g.stars[i].accel[1] < -accelCap)
                    {
                        g.stars[i].accel[1] = -accelCap;
                    }

                    

                    g.stars[i].force[0] = 0;
                    g.stars[i].force[1] = 0;
                }
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Galaxy g in galaxies)
            {
                
                foreach (Star s in g.stars)
                {
                    if (!s.isCore)
                    {
                        e.Graphics.DrawEllipse(starPen, (Convert.ToInt64(s.x) + camX) / camScale, (Convert.ToInt64(s.y) + camY) / camScale, 1, 1);
                    }
                    else
                    {
                        e.Graphics.DrawEllipse(redPen, (Convert.ToInt32(s.x) + camX) / camScale, (Convert.ToInt32(s.y) + camY) / camScale, 1, 1);
                    }
                }

            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    pressedKeys[0] = true;
                    break;
                case Keys.S:
                    pressedKeys[1] = true;
                    break;
                case Keys.A:
                    pressedKeys[2] = true;
                    break;
                case Keys.D:
                    pressedKeys[3] = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    pressedKeys[0] = false;
                    break;
                case Keys.S:
                    pressedKeys[1] = false;
                    break;
                case Keys.A:
                    pressedKeys[2] = false;
                    break;
                case Keys.D:
                    pressedKeys[3] = false;
                    break;
            }
        }
    }

    public class Galaxy
    {
        public bool speedToggle;
        public bool hasCore;
        public int mass;
        public int starCount;
        public int[] loc = new int[2];
        public List<Star> stars = new List<Star>();
        public Galaxy(int bodies, int mass, bool hasCore, bool speedToggle, float min, float density)
        {
            int starMass = mass / bodies;
            this.loc[0] = 400;
            this.loc[1] = 225;
            this.starCount = bodies;
            GenerateStars(starMass, bodies, this.loc[0], this.loc[1], hasCore, min, density);
            this.hasCore = hasCore;
            this.mass = mass;
            this.speedToggle = speedToggle;
        }
        public Galaxy(int bodies, int mass, int x, int y, bool hasCore, bool speedToggle, float min, float density)
        {
            int starMass = mass / bodies;
            this.loc[0] = x;
            this.loc[1] = y;
            this.starCount = bodies;
            GenerateStars(starMass, bodies, this.loc[0], this.loc[1], hasCore, min, density);
            this.hasCore = hasCore;
            this.mass = mass;
            this.speedToggle = speedToggle;
        }
        public void GenerateStars(int starMass, int count, int galX, int galY, bool hasCore, float min, float density)
        {
            Random r = new Random();
            double[] avgLoc = new double[2];
            for (int i = 0; i < count; i++)
            {
                int[] loc = weightedRandLoc(this.loc[0],this.loc[1], r, min, density);
                //int x = weightedRandLoc(this.loc[0], r);
                //int y = weightedRandLoc(this.loc[1], r);
                Star s = new Star(starMass, loc[0], loc[1], this, false);
                avgLoc[0] += loc[0];
                avgLoc[1] += loc[1];
                stars.Add(s);
            }
            if (hasCore)
            {
                avgLoc[0] /= stars.Count();
                avgLoc[1] /= stars.Count();
                Star core = new Star((int)(starMass * count * 0.25), Convert.ToInt32(avgLoc[0]), Convert.ToInt32(avgLoc[1]), this, true);
                stars.Add(core); // Uncomment for galactic core
            }
            
        }

        public int[] weightedRandLoc(int i, int j, Random random, float min, float density)
        {
            //double min = 0;
            double max = 2 - min;
            //random.NextDouble() * (maximum - minimum) + minimum;, thanks StackOverflow
            int[] nums =
            {
                weightedRandNum(i, random, min, 0, density), weightedRandNum(j, random, min, 0, density)
            };
            int[] output = new int[2];
            while (Math.Sqrt(Math.Pow(Math.Abs(nums[0]), 2) + Math.Pow(Math.Abs(nums[1]), 2)) <= max)
            {
                nums[0] = weightedRandNum(i, random, min, 0, density);
                nums[1] = weightedRandNum(j, random, min, nums[0], density);
                
            }
            output[0] = Convert.ToInt32(nums[0]);
            output[1] = Convert.ToInt32(nums[1]);
            return output;
        }

        public int weightedRandNum(int i, Random random, double min, double x, float density)
        {
            int z = i;
            i = random.Next((int)(-this.starCount / density) - 1, (int)(this.starCount / density) + 1);
            //random.NextDouble() * (maximum - minimum) + minimum;, thanks StackOverflow
            //double min = 0.5;
            double max = 2 - min;
            double unweighted = random.NextDouble() * (max - min) + min;
            int odds = Convert.ToInt32(unweighted * 100);
            int output;

            if (random.Next(1, 100) > Math.Abs(odds - 100))
            {
                output = Convert.ToInt32(unweighted * i);
                if (Math.Sqrt(Math.Pow(Math.Abs(output), 2) + Math.Pow(Math.Abs(x), 2)) >= max)
                {
                    
                    return output + z;
                }
                else
                {
                    return weightedRandNum(z, random, min, x, density);
                }
                
            }
            else
            {
                return weightedRandNum(z, random, min, x, density);
            }
        }


    }
    public class Star
    {
        public int interactedMass = 0;
        public double x;
        public double y;
        public int mass;
        public bool isCore;
        public double[] speed = new double[2];
        public double[] accel = new double[2];
        public double[] force = new double[2];
        //public Galaxy galaxy;

        public Star(int mass, int X, int Y, Galaxy g, bool isCore)
        {
            this.isCore = isCore;
            this.x = X;
            this.y = Y;
            this.mass = mass;
            /*if (x > 400)
            {
                mass *= Convert.ToInt32(400 / x);
            }
            else
            {
                mass *= Convert.ToInt32(x / 400);
            }

            if (y > 225)
            {
                mass *= Convert.ToInt32(225 / y);
            }
            else
            {
                mass *= Convert.ToInt32(y / 225);
            }*/
            if (!isCore && g.speedToggle)
            {
                if (this.x > g.loc[0])
                {
                    this.speed[1] = (this.x - g.loc[0]) * 0.2 * Math.Cbrt(mass) / 12;
                }
                else
                {
                    this.speed[1] = -(g.loc[0] - this.x) * 0.2 * Math.Cbrt(mass) / 12;
                }

                if (this.y > g.loc[1])
                {
                    this.speed[0] = -(this.y - g.loc[1]) * 0.2 * Math.Cbrt(mass) / 12;
                }
                else
                {
                    this.speed[0] = (g.loc[1] - this.y) * 0.2 * Math.Cbrt(mass) / 12;
                }
            }
            
            //this.galaxy = g;
        }
    }
}

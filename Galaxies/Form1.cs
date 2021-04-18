using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galaxies
{
    public partial class Form1 : Form
    {
        double G = 0.0000000000667; //0.0000000000667
        List<Galaxy> galaxies = new List<Galaxy>();   
        Pen starPen = new Pen(Color.White, 1);
        Pen redPen = new Pen(Color.OrangeRed, 1);
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Galaxy g = new Galaxy(200, 10000);
            galaxies.Add(g);
        }
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            foreach(Galaxy g in galaxies)
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
                        double dist = Math.Pow(Math.Sqrt(Math.Pow(g.stars[i].x, 2) + Math.Pow(g.stars[i].y, 2)) - Math.Sqrt(Math.Pow(g.stars[j].x, 2) + Math.Pow(g.stars[j].y, 2)), 2);
                        if (dist == 0)
                        {
                            dist = Math.Pow(Math.Sqrt(Math.Pow(g.stars[i].x, 2) + Math.Pow(g.stars[i].y, 2)) - Math.Sqrt(Math.Pow(g.stars[j].x - 1, 2) + Math.Pow(g.stars[j].y + 1, 2)), 2);
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
                        /*if (g.stars[i].x < g.stars[j].x)
                        {
                            xForce = -xForce;
                        }
                        if (g.stars[i].y < g.stars[j].y)
                        {
                            yForce = -yForce;
                        }*/
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
                            int xad = 0;
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
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Galaxy g in galaxies)
            {
                
                foreach (Star s in g.stars)
                {
                    if (!s.isCore)
                    {
                        e.Graphics.DrawEllipse(starPen, Convert.ToInt64(s.x), Convert.ToInt64(s.y), 1, 1);
                    }
                }
                e.Graphics.DrawEllipse(redPen, Convert.ToInt32(g.stars[g.stars.Count - 1].x), Convert.ToInt32(g.stars[g.stars.Count - 1].y), 1, 1);
            }
            
        }

        
    }

    public class Galaxy
    {
        public int[] loc = new int[2];
        public List<Star> stars = new List<Star>();
        public Galaxy(int bodies, int mass)
        {
            int starMass = mass / bodies;
            this.loc[0] = 400;
            this.loc[1] = 225;
            GenerateStars(starMass, bodies, this.loc[0], this.loc[1]);
        }
        public void GenerateStars(int starMass, int count, int galX, int galY)
        {
            Random r = new Random();
            double[] avgLoc = new double[2];
            for (int i = 0; i < count; i++)
            {
                int[] loc = weightedRandLoc(this.loc[0],this.loc[1], r);
                //int x = weightedRandLoc(this.loc[0], r);
                //int y = weightedRandLoc(this.loc[1], r);
                Star s = new Star(starMass, loc[0], loc[1], this, false);
                avgLoc[0] += loc[0];
                avgLoc[1] += loc[1];
                stars.Add(s);
            }
            avgLoc[0] /= stars.Count();
            avgLoc[1] /= stars.Count();
            Star main = new Star(starMass * count * 10000000, Convert.ToInt32(avgLoc[0]), Convert.ToInt32(avgLoc[1]), this, true);
            stars.Add(main);
        }

        public int[] weightedRandLoc(int i, int j, Random random)
        {
            double min = 0;
            double max = 2 - min;
            //random.NextDouble() * (maximum - minimum) + minimum;, thanks StackOverflow
            int[] nums =
            {
                weightedRandNum(i, random, min, 0), weightedRandNum(j, random, min, 0)
            };
            int[] output = new int[2];
            while (Math.Sqrt(Math.Pow(Math.Abs(nums[0]), 2) + Math.Pow(Math.Abs(nums[1]), 2)) <= max)
            {
                nums[0] = weightedRandNum(i, random, min, 0);
                nums[1] = weightedRandNum(j, random, min, nums[0]);
                
            }
            output[0] = Convert.ToInt32(nums[0]);
            output[1] = Convert.ToInt32(nums[1]);
            return output;
        }

        public int weightedRandNum(int i, Random random, double min, double x)
        {
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
                    
                    return output;
                }
                else
                {
                    return weightedRandNum(i, random, min, x);
                }
                
            }
            else
            {
                return weightedRandNum(i, random, min, x);
            }
        }


    }
    public class Star
    {
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
            this.x = X;
            this.y = Y;
            this.mass = mass;
            if (x > 400)
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
            }
            if (!isCore)
            {
                if (this.x > g.loc[0])
                {
                    this.speed[1] = (this.x - g.loc[0]) * 0.2 * Math.Cbrt(mass) / 750;
                }
                else
                {
                    this.speed[1] = -(g.loc[0] - this.x) * 0.2 * Math.Cbrt(mass) / 750;
                }

                if (this.y > g.loc[1])
                {
                    this.speed[0] = -(this.y - g.loc[1]) * 0.2 * Math.Cbrt(mass) / 750;
                }
                else
                {
                    this.speed[0] = (g.loc[1] - this.y) * 0.2 * Math.Cbrt(mass) / 750;
                }
            }
            
            //this.galaxy = g;
        }
    }
}

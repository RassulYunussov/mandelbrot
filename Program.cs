using System;
using SixLabors.ImageSharp;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mandelbrot
{
    class Program
    {
        static void Main(string[] args)
        {
            int height = 800;
            int width = 800;
            int max = 10000;
            Stopwatch sw = new Stopwatch();
            using(Image<Rgba32> img = new Image<Rgba32>(width,height))
            {
                sw.Start();
                List<Task> tasks = new List<Task>();
                for (int row = 0; row < height; row++) {
                    for (int col = 0; col < width; col++) {
                        tasks.Add(Task.Factory.StartNew(o=>{
                            int x = (o as Tuple<int,int>).Item2;
                            int y = (o as Tuple<int,int>).Item1;
                            double c_re = (x - width/2.0)*4.0/width;
                            double c_im = (y - height/2.0)*4.0/width;
                            double xx = 0, yy = 0;
                            int iteration = 0;
                        
                            while (xx*xx+yy*yy <= 4 && iteration < max) {
                                double x_new = xx*xx - yy*yy + c_re;
                                yy = 2*xx*yy + c_im;
                                xx = x_new;
                                iteration++;
                            }
                            
                            Rgba32 color = new Rgba32((uint)(uint.MaxValue*iteration/max));
                            if (iteration < max) img[x,y] = color;
                            else img[x,y] = Rgba32.Black;

                        },new Tuple<int,int>(row,col)));
                    }
                }
                Task.WaitAll(tasks.ToArray());
                sw.Stop();
                System.Console.WriteLine(sw.ElapsedMilliseconds);
                img.Save("mand.jpg");
            }
        }
    }
}

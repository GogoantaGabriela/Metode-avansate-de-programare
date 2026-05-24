using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::DrawingTool.Canvas;
using global::DrawingTool.Composite;
using global::DrawingTool.Interfaces;
using global::DrawingTool.Models;
using global::DrawingTool.Proxy;
using global::DrawingTool.Shapes;

namespace DrawingTool
{
    class Program
        {
                static void Main(string[] args)
                {
                    ICanvas console = new ConsoleCanvas();
                    SvgCanvas svg = new SvgCanvas();

                    Picture face = new Picture();
                    face.Add(new Circle(10, 10, 5));  
                    face.Add(new Circle(25, 10, 5));  
                    face.Add(new Line(10, 25, 25, 25)); 

                   
                    Picture scene = new Picture();
                    scene.Add(face);
                    scene.Add(new Rectangle(5, 5, 30, 40)); 
                    scene.Add(new Line(0, 100, 200, 100)); 

                   
                    IShape lockedCircle = new ReadOnlyShapeProxy(new Circle(100, 100, 20));
                    scene.Add(lockedCircle);

                  
                    Console.WriteLine("=== AFISARE SCENA INITIALA (BRIDGE: Console) ===");
                    scene.Draw(console);

                    Console.WriteLine("\n=== TESTARE TRANSFORMARI (COMPOSITE) ===");
                    
                    Console.WriteLine("Mutam scena cu (10, 10)...");

                    try
                    {
                    
                        scene.Move(10, 10);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("Proxy activat: " + ex.Message);
                    }

                   
                    Console.WriteLine("\n=== EXPORT SVG (BRIDGE: SvgCanvas) ===");
                    face.Draw(svg);
                    Console.WriteLine(svg.GetSvg());

                    BoundingBox box = face.GetBoundingBox();
                    Console.WriteLine("\n=== METADATE BOUNDING BOX (Fata robot) ===");
                    Console.WriteLine(string.Format("Limita Stanga-Sus: ({0}, {1})", box.Xmin, box.Ymin));
                    Console.WriteLine(string.Format("Limita Dreapta-Jos: ({0}, {1})", box.Xmax, box.Ymax));

                    Console.WriteLine("\nApasati orice tasta pentru a termina...");
                    Console.ReadKey();
                }
            }
        }

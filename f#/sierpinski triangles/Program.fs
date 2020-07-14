// Learn more about F# at http://fsharp.org

open System
open System.Drawing;
open System.Diagnostics

//These are the three configurable parameters. The script will 
// generate a sierpinski triangle with size configured in the detail
// parameter. Image size defines the scaling
let imagePath = "./output.bmp";
let detail = 256.0;
let imageSize = 2048;

type point = {x:float; y:float}
type triangle = {height:float; center_x: float; center_y:float; pointA: point; pointB: point; pointC: point}

//creates an inverted triangle. the center is the tip of the inverted triangle. 
//meaning it is at the middlepoint
let V h (centerX,centerY) = { 
    height = h; 
    center_x = centerX; 
    center_y = centerY;
    pointA= {x = 0.0; y= 0.0}; 
    pointB= {x = h/2.0; y = 0.0}; 
    pointC = {x = h/4.0;y = h/2.0 * -1.0}
}

//Translate a triangle by X and Y units
let T x y triangle = {
    triangle with 
        pointA = {x = triangle.pointA.x + x; y = triangle.pointA.y + y};
        pointB = {x = triangle.pointB.x + x; y = triangle.pointB.y + y};
        pointC = {x = triangle.pointC.x + x; y = triangle.pointC.y + y}
    }

//Scale a triangle from a scalar
let S scalar triangle = {
    triangle with 
        pointA = {x = triangle.pointA.x * scalar; y = triangle.pointA.y * scalar};
        pointB = {x = triangle.pointB.x * scalar; y = triangle.pointB.y * scalar};
        pointC = {x = triangle.pointC.x * scalar; y = triangle.pointC.y * scalar}
    }

//Generates the triangles recursively. This method generates the inverted triangles.
//By connecting the generated points we get the other triangles. This is a recursive
//tree.
let rec generate level height origX origY=
    if ((2.0 ** (level + 1.0)) > detail) then  //stop condition. if it is too detailed it will stop yooooo
        []
    else 
        let mutable curHeight = height;
        let mutable points = [];
        let mutable centerY = origY;
        for i in 0 .. int (System.Math.Log(height,2.0)) do
            let dx = origX - curHeight/4.0;
            let dy = centerY + curHeight/2.0;
            
            //create current
            let children = generate (level + 1.0) (curHeight/2.0) (origX + curHeight/4.0) centerY ; //recurse
            let current =   V curHeight (origX, centerY)
                            |> T dx dy
            
            //mirror all children
            points <- points @ (children |> List.map (fun triangle -> triangle |> T (-curHeight/2.0) 0.0))
            points <- points @ current :: children
            
            centerY <- centerY + curHeight/2.0
            curHeight <- curHeight / 2.0;
        points

let printSierpinski size triangleSize triangles  =
    //Translate the triangles to y=height and x=0.
    let translate t = t |> S (float size/triangleSize)
                        |> T (float size/2.0) (float size)

    let image = new Bitmap(size, size);
    let graph = Graphics.FromImage(image);
    let pen = new Pen(Brushes.Black);

    graph.Clear(Color.Azure);
    
    for triangle in triangles |> List.map translate do
        graph.DrawLines (pen, [ 
            Point(int triangle.pointA.x, size - int triangle.pointA.y); 
            Point(int triangle.pointB.x, size - int triangle.pointB.y); 
            Point(int triangle.pointC.x, size - int triangle.pointC.y); 
            Point(int triangle.pointA.x, size - int triangle.pointA.y);
            Point(int triangle.pointC.x, size - int triangle.pointC.y); 
            Point(int triangle.pointB.x, size - int triangle.pointB.y);  ] 
        |> Array.ofList)
            // )
    graph.DrawLines (pen, [ Point(size/2, 0); Point(0, size); Point(size, size); Point(size/2,0) ] |> Array.ofList);
    image.Save(imagePath);
    

[<EntryPoint>]
let main argv =
    let sw = Stopwatch.StartNew();
    let points = generate 0.0 detail 0.0 -detail;
    printfn "generation took %d ms" sw.ElapsedMilliseconds;

    points
    |> printSierpinski imageSize detail;
    0






    //reserve a string big enough. Take into account the \r\n per line too
    //translate the triangle to the middle of the string such that the left-down vertex is x=max/y=max
    //
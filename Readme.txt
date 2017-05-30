Bernd van den Hoek - 5895391
Maaike Galama - 5987857
Wilmer Zwietering - 5954312

Bonus assignments implemented:
- Anti-aliasing.
RayTracer.cs lines 54 -75
To be found in RayTracer.Render() method. 
How much anti-aliasing is done can be set by the member variable raysPerPixel. For the number of rays per pixel a random ray is cast within the bounds of that pixel. The values of Trace() for each ray are divided by the number of rays per pixel, the sum of which is the pixel color.

- Refraction
Raytracer.cs from line 167
To be found in DoFancyColorCalculation(), GetSchlickReflection(), and Refract().
Schlicks approximation is used for fresnel effect.

- Absorbtion
RayTracer.cs lines 124 - 129
To be found in RayTracer.Trace().

- Skydome
RayTracer.cs lines 39-41, 85-86, 135-153
Scene.cs lines 13, 18
The skydome uses the beautiful palace of Uffizi as background. The image was retrieved from http://www.pauldebevec.com/Probes/ and converted to a from a 48bit HDR to a 24bit PNG. The equation to calculate the background pixel was also found on the aforementioned website and converted to C#. The y-axis is flipped, because our world uses the y = 0 as bottom of the screen, but the C# uses y = 0 as top of the screen. To optimise the speed of the color retrieval we converted the bitmap to an array with color data.

Materials used:
Powerpoint and lecture notes from the amazing UU graphics teachers
Stackoverflow (of course).
http://www.lighthouse3d.com/tutorials/maths/ray-sphere-intersection/
https://graphics.stanford.edu/courses/cs148-10-summer/docs/2006--degreve--reflection_refraction.pdf
http://www.pauldebevec.com/Probes/

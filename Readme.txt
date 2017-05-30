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
WILMER HIER UITLEG DOEN

Materials used:
Stackoverflow (of course).
http://www.lighthouse3d.com/tutorials/maths/ray-sphere-intersection/
https://graphics.stanford.edu/courses/cs148-10-summer/docs/2006--degreve--reflection_refraction.pdf
http://www.pauldebevec.com/Probes/

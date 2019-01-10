This is an all-sky cubemap from the DSS2 Digitized Sky Survey
	- Fetched from sky-map.org 2011-01-16 with the script below.
	- Manually cleaned (removed stripes, reduced some image block boundaries)

It forms the six faces of a continuous OpenGL cubemap, showing the milky way and all major constellations.  There isn't a green channel, but it looks pretty good anyway.

Prepared by Dr. Orion Sky Lawlor 2011-01-16, and released to the public domain.

----- Data Source -----
More information on the Digitized Sky Survey:
	http://en.wikipedia.org/wiki/Digitized_Sky_Survey
	http://archive.stsci.edu/dss/

Acknowledgement info for the original DSS2 source data (my modifications are released to the public domain):
"The Digitized Sky Surveys were produced at the Space Telescope Science Institute under U.S. Government grant NAG W-2166. The images of these surveys are based on photographic data obtained using the Oschin Schmidt Telescope on Palomar Mountain and the UK Schmidt Telescope. The plates were processed into the present compressed digital form with the permission of these institutions.

The National Geographic Society - Palomar Observatory Sky Atlas (POSS-I) was made by the California Institute of Technology with grants from the National Geographic Society.

The Second Palomar Observatory Sky Survey (POSS-II) was made by the California Institute of Technology with funds from the National Science Foundation, the National Geographic Society, the Sloan Foundation, the Samuel Oschin Foundation, and the Eastman Kodak Corporation.

The Oschin Schmidt Telescope is operated by the California Institute of Technology and Palomar Observatory.

The UK Schmidt Telescope was operated by the Royal Observatory Edinburgh, with funding from the UK Science and Engineering Research Council (later the UK Particle Physics and Astronomy Research Council), until 1988 June, and thereafter by the Anglo-Australian Observatory. The blue plates of the southern Sky Atlas and its Equatorial Extension (together known as the SERC-J), as well as the Equatorial Red (ER), and the Second Epoch [red] Survey (SES) were all taken with the UK Schmidt. "


------ Aquisition instructions -------
To work around a bug in the "imgcut" script (black scoops in top and bottom of images), this data is rotated about the Y axis by 5 degrees.  This makes the "Zp.jpg" not quite pointing at the celestial north pole, but down 5 degrees from it.  Lining all this up was quite a job!

wget -O Xp.jpg 'http://server4.sky-map.org/imgcut?survey=DSS2&img_id=all&angle=114&ra=0.0&de=5.0&width=2048&height=2048&img_borders=&projection=tan&rotation=-90.0&reverse=0&interpolation=bicubic&jpeg_quality=0.9&output_type=jpeg'
wget -O Ym.jpg 'http://server4.sky-map.org/imgcut?survey=DSS2&img_id=all&angle=114&ra=6.0&de=0.0&width=2048&height=2048&img_borders=&projection=tan&rotation=5.0&reverse=0&interpolation=bicubic&jpeg_quality=0.9&output_type=jpeg'
wget -O Xm.jpg 'http://server4.sky-map.org/imgcut?survey=DSS2&img_id=all&angle=114&ra=12.0&de=-5.0&width=2048&height=2048&img_borders=&projection=tan&rotation=90.0&reverse=0&interpolation=bicubic&jpeg_quality=0.9&output_type=jpeg'
wget -O Yp.jpg 'http://server4.sky-map.org/imgcut?survey=DSS2&img_id=all&angle=114&ra=18.0&de=0.0&width=2048&height=2048&img_borders=&projection=tan&rotation=-185.0&reverse=0&interpolation=bicubic&jpeg_quality=0.9&output_type=jpeg'
wget -O Zp.jpg 'http://server4.sky-map.org/imgcut?survey=DSS2&img_id=all&angle=114&ra=180.0&de=85.0&width=2048&height=2048&img_borders=&projection=tan&rotation=90.0&reverse=0&interpolation=bicubic&jpeg_quality=0.9&output_type=jpeg'
wget -O Zm.jpg 'http://server4.sky-map.org/imgcut?survey=DSS2&img_id=all&angle=114&ra=0.0&de=-85.0&width=2048&height=2048&img_borders=&projection=tan&rotation=-90.0&reverse=0&interpolation=bicubic&jpeg_quality=0.9&output_type=jpeg'

convert Xp.jpg Xm.jpg Yp.jpg Ym.jpg Zp.jpg Zm.jpg -append stars.jpg

These versions have been manually edited to remove lines and frame edges, and improve color balance.

"Zp" faces toward the north star.


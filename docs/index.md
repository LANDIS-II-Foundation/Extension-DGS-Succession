# What is the DGS Extension?

Here we developed a new succession extension (DGS) of the LANDIS-II forest landscape model which integrates a vegetation dynamics model (NECN) with a soil carbon model (DAMM-McNiP), a hydrologic model (SHAW), and a deep soil profile permafrost model (GIPL) in a spatially-explicit framework. DGS Succession uses the algorithms in the NECN Succession extension of LANDIS-II to simulate growth, mortality and reproduction of vegetation but has three major changes. 
1) The simple bucket model in NECN was replaced with a physically-based model (SHAW) that simulates energy and water fluxes (e.g. snow depth, evapotranspiration, soil moisture) at multiple levels in the canopy and soil. 
2) The active, slow, and passive soil pools in NECN were replaced by seven soil pools that are measurable in the field, with carbon and nitrogen dynamics dictated by DAMM-McNiP. 
3) Soil temperature and soil moisture are simulated only at one depth in NECN, but in DGS, soil temperature (and hence permafrost dynamics) are simulated at as many as 50 user-defined depths down to 4 m with SHAW and 75 m with GIPL.

# Citation

Melissa S. Lucash, Adrienne M. Marshall, Shelby A. Weiss, John W. McNabb, Dmitry J. Nicolsky, Gerald N. Flerchinger, Timothy E. Link, Jason G. Vogel, Robert M. Scheller, Rose Z. Abramoff, Vladimir E. Romanovsky. 2023. Burning trees in frozen soil: Simulating fire, vegetation, soil, and hydrology in the boreal forests of Alaska, Ecological Modelling, Volume 481, 110367, https://doi.org/10.1016/j.ecolmodel.2023.110367.

# Release Notes

- Latest release: Version 2.0 July 8, 2025
- Full release details found in the User Guide and on GitHub.
- [View User Guide](https://github.com/LANDIS-II-Foundation/Extension-DGS-Succession/blob/master/docs/LANDIS-II%20DGS%20Succession%20v1.0%20User%20Guide.pdf).
- DGS depends on the Climate Library, see: [User Guide for Climate Library](https://github.com/LANDIS-II-Foundation/Library-Climate/blob/master/docs/LANDIS-II%20Climate%20Library%20v4.2%20User%20Guide.pdf)

- Copyright: The LANDIS-II Foundation

# Requirements

You need:

- The [LANDIS-II model v8.0](http://www.landis-ii.org/install) installed on your computer.
- Example files (see below)

# Download

- The latest version of DGS can be [downloaded from GitHub](https://github.com/LANDIS-II-Foundation/Extension-DGS-Succession/blob/master/deploy/installer/LANDIS-II-V7%20DGS%20Succession%201.102-setup.exe). To install it on your computer, launch the installer.

# Example Files

LANDIS-II requires a global parameter file for your scenario, and separate parameter files for each extension.

Example files can be [downloaded from GitHub](https://downgit.github.io/#/home?url=https://github.com/LANDIS-II-Foundation/Extension-DGS-Succession/tree/master/testing/Core7-DGS_version1.102).


# Support

If you have a question, please contact Dr. Melissa Lucash. 
You can also ask for help in the [LANDIS-II users group](http://www.landis-ii.org/users).

If you come across any issue or suspected bug, please post about it in the [issue section of the Github repository](https://github.com/LANDIS-II-Foundation/Extension-DGS-Succession/issues) (GitHub ID required).

# Author
[Melissa Lucash](https://melissalucash.com)


# AdsbMon

ADSB Monitoring by decoding raw output from Dump1090. This is self-hostable software that connects
to a [Dump1090](https://github.com/antirez/dump1090) socket on your network, decodes ADSB messages from aircraft present in the local area, 
and then makes information about the aircraft available via API routes. 
It only supports the extended squitter messages with a 112-bit data block. [Dump1090](https://github.com/antirez/dump1090) may send
more messages that have the shorter 56-bit data block, these are not supported.

The following ADSB messages are supported:
- Aircraft Identification
- Airborne Position
- Airborne Velocity

The messages that are currently not supported are:
- Surface position
- Operation status

I haven't written any support for surface position yet because, due to location (e.g. not directly on an airport), 
I am very very unlikely to receive any ADSB messages from aircraft on the ground. 
The operation status messages aren't supported mostly just because they don't contain information that is all that 
useful or interesting to me. If you need support for these, feel free to implement it and make a PR.

You can learn more about ADSB message types and how they are encoded by reading _The 1090 Megahertz Riddle_ by Junzi Sun:
https://mode-s.org/decode/index.html

## Usage Recommendations

Right now I recommend using the API project and then writing some kind of frontend that supports your use-case. It 
already has a Dockerfile for easily building a Docker image and even a compose.yml file that pulls the latest image
that is built off of the main branch of this repo.

The Web project has what is basically just a proof of concept. If you want to use that, you'll have to create a Mapbox
account and put your public token in the Index.cshtml file.

I don't have any documentation for the API project yet so you'll have to read the code and experiment to figure it out.
However, there is only one route (`/aircraft`) as of writing this so that shouldn't be too difficult.

I'm also working separately on a frontend that may eventually make it into this project.

## Recommended Hardware

If you ran across this repository and you're wondering what you need to get started with monitoring ADSB messages in 
your area, I can only recommend what I have tried. Currently my setup consists of a Raspberry Pi running a lightweight,
headless distro (e.g. Ubuntu Server) connected to a RTL-SDR Blog V4 software defined radio. I'm currently just using
the dipole antenna included in [this kit](https://www.amazon.com/dp/B0CD7558GT).

I do have the dipole antenna configured in what I think is probably the most optimal way I can. I use the shortest two
antennas from the kit connected to the coaxial adapter. I used the suction cup mount to stick it to the inside of my
window. The antennas I have collapsed all the way to fit as closely to the wavelength necessary and I orientated them
vertically. With this setup I get fairly good range for higher altitude aircraft, especially commercial airliners.

I obviously don't really know what I'm talking about when it comes to antennas, but there are plenty of people who
have posted to forums and blogs about this subject that I have discovered. Just look up antenna configuration for ADSB and you
should be able to find some good resources.

For a more expensive, but easier setup and probably better range, I recommend looking into [FlightAware hardware](https://flightaware.store).
I plan on upgrading my setup with their antenna and filter at some point.

# PMSDetect
I made this thing years ago to track Plex Media Player with [taiga](https://files.catbox.moe/cmpyq2.png). Requires a modified taiga version.


Honestly I don't remember half the things I did in this, but hey, it has worked for me for the past few years so at least it's something ¯\\\_(ツ)_/¯

You need to create a settings.xml and put these things in it:

```
<?xml version="1.0" encoding="utf-8"?>
<root>
  <baseURL>*plex server url*</baseURL>
  <username>*plex username*</username>
  <server>*plex server name*</server>
  <machineIdentifier>*machineid*</machineIdentifier>
  <token>*token*</token>
</root>
```

iirc you can find the token and stuff like this: https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/

This essentially just runs a window that will grab whatever plex is playing like this:

![Not playing](https://files.catbox.moe/dj6cb9.png)
![Playing](https://files.catbox.moe/z1i0im.png)

Honestly I don't even know/remember if you need to have your PMP and PMS on the same system as this program so ymmv.

Also, to have Taiga actually detect this thing you'll have to add this program as a player:

```
Plex Media Server
	windows:
		^WindowsForms10\.Window\.8\.app\..+
	executables:
		PMSDetect
	strategies:
		window_title:
			^(.+)
```

in `deps/src/anisthesia/data/players.anisthesia` 

It'll then appear in the list of media players

![Modified taiga](https://files.catbox.moe/rmhtbe.png)

Pretty sure conceptually you could run Plex Media Server and taiga on the same device, and detect the currently playing video per user or something like that, but here's at least everything working for this specific usage that I needed it for:


![Everything together](https://files.catbox.moe/cmpyq2.png)


I may or may not make a better readme in the future.

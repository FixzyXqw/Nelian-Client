# Nelian Client

**Nelian Client** is a lightweight, performance focused Minecraft Client and Launcher.

## Why Nelian?

There are Minecraft clients with larger teams, more resources, and longer feature lists than Nelian and that's completely fine. Nelian isn't trying to compete on scale; it simply follows a different approach.

We believe software should serve the people who use it, without unnecessary features, intrusive ads, or anything designed primarily to extract value from the user. Every feature in Nelian exists for a reason: to make Minecraft more enjoyable, more comfortable, or more performant. Nelian isn't built to generate revenue. It's built to offer a clean, fast, and enjoyable way to play Minecraft.

## Repository Scope

This repository contains the source code for custom classes developed and modified specifically for Nelian Client, including client-side features, performance optimizations, rendering improvements, and other custom systems.

> **Note:** This repository does not contain the complete source code of Minecraft. It includes only the classes created, modified, or adapted for Nelian Client, and does not represent Minecraft's full source.

## License

The license included in this repository applies to the open-source content it contains.

Following the first official release of Nelian Client, the classes in this repository will no longer receive updates. The repository will remain publicly available for reference and transparency, but the source files will not be actively maintained afterward.

## Launcher & Privacy

The launcher component of this project relies primarily on the [Cmllib](https://github.com/CmlLib) project.

Added Microsoft accounts, usernames, and UUID information are never sent to or stored on any external server, aside from Microsoft's own servers. Both offline and online account data are stored locally on your device.
## Security & Ban System

### Why does a lightweight launcher include a ban system?

Nelian does **not** monitor your gameplay, collect gameplay data, or issue bans for your in-game actions. We have no interest in controlling how you play Minecraft or restricting your access to multiplayer servers.

The purpose of Nelian's ban system is solely to protect the integrity of the Nelian project itself. A ban is only issued when the launcher or client detects attempts to attach debuggers, use exploits, tamper with security-related components, or otherwise interfere with Nelian's protection mechanisms.

The penalty system is progressive:

* **First violation:** 24-hour suspension
* **Second violation:** 7-day suspension
* **Third violation:** Permanent ban

Our goal is simple: we provide a privacy-focused, ad-free experience without collecting unnecessary user data, and in return we ask that users respect the integrity of the project by not attempting to tamper with its security.

To maintain transparency, the custom source code developed specifically for Nelian is published publicly on GitHub in a read-only form for educational purposes. However, proprietary Nelian security systems, third-party libraries (including CmlLib), and any Minecraft source code are **not** included in this repository.


## Disclaimer

Minecraft is a trademark of Mojang Studios. Nelian Client is an independent project, developed separately from and not affiliated with, endorsed by, or sponsored by Mojang Studios or Microsoft.

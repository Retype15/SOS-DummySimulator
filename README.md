# S.O.S. - Dummy Simulator

[![GitHub – Download](https://img.shields.io/badge/GitHub-Download-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Retype15/SOS-DummyManager/releases/latest/download/SOS-DummyManager.zip)
[![Steam Workshop](https://img.shields.io/badge/Steam_Workshop-Add_here-1B2838?style=for-the-badge&logo=steam&logoColor=white)](https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282)

---

**S.O.S. - Dummy Simulator** is the clinical training and medical simulation satellite plugin for **S.O.S. (Standard Operations Schematics)** in **Barotrauma**. Designed for doctors, combat medics, and modders, it integrates a fully interactive test patient directly into your S.O.S. inspector window—allowing you to experiment with lethal afflictions, test experimental treatments, and observe disease progression in real time without wasting precious medical supplies or endangering your crew.

[![Add from Workshop](https://img.shields.io/badge/Add_From-Steam_Workshop-1B2838?style=for-the-badge&logo=steam&logoColor=white)](https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282)

[![Manually Download Latest Version](https://img.shields.io/badge/Manually_Download_Latest_Version-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Retype15/SOS-DummyManager/releases/latest/download/SOS-DummyManager.zip)

## Key Features

- **Interactive Clinical Simulator:** Seamlessly embeds Barotrauma's native medical inspection interface inside S.O.S. whenever an Item or Affliction is selected.
- **Zero-Risk Medical Testing:** Safely diagnose conditions, inject modded toxins, test surgical procedures, and practice treatments without consuming actual inventory items.
- **Precision Affliction Control:** Inject any affliction onto specific limbs or the whole body, fine-tune strength with live sliders, instantly set maximum strength (100%), or cure conditions on demand.
- **Mock Item Application:** Simulate using any medical item or weapon on individual limbs to observe immediate biological effects and degree of success.
- **Time Manipulation:** Control biological simulation time with Play/Pause and variable time acceleration (**x1, x2, x5, x10**) to observe how afflictions worsen, stabilize, or heal naturally over time.
- **Interactive Body Navigation:** Click on any active affliction on the patient to inspect its full S.O.S. documentation, or right-click to modify its intensity on the fly.
- **Mortality Tracker:** Tracks cumulative dummy casualties across your campaign with persistent patient state.

## Requirements

This mod is an official extension and requires the following base mods installed and enabled:

1. **[LuaCsForBarotrauma](https://github.com/evilfactory/LuaCsForBarotrauma)** (And enable C# Scripting).
2. **[S.O.S. - Standard Operations Schematics](https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282)** (Core).

## Controls & Usage

### Inside S.O.S

1. Open **S.O.S.** (default hotkey: **[J]**).
2. Select any **Item** or **Affliction** in the browser.
3. Switch to the **Simulator** tab in the central panel.

### Simulation Interactions

- **[Right Click on a Limb]**: Opens the simulation context menu for the targeted limb, allowing you to inject the currently selected affliction or apply the selected medical item.
- **[Left Click on an Affliction Icon]**: Navigates S.O.S. directly to that affliction's documentation page.
- **[Right Click on an Affliction Icon]**: Opens the strength slider menu to increase, reduce, or remove that specific affliction.
- **[START / PAUSE]**: Freezes or resumes biological updates and affliction progression.
- **[Speed Dropdown]**: Accelerates simulation time (x1, x2, x5, x10) to observe long-term affliction effects.
- **[RESET]**: Reverts the patient's health and afflictions to the snapshot taken before the simulation was started.
- **[DROP]**: Discards the current patient, records a casualty in your statistics, and spawns a fresh, healthy dummy subject.

## Common questions

**Q:** *Can it be used on multiplayer and vanilla servers?*

- **A:** Yes! Just like S.O.S., this is a client-side utility mod. It runs entirely on your client and does not require the server or other players to have it installed.

**Q:** *Does simulating items consume items from my inventory?*

- **A:** Absolutely not. The simulator spawns temporary mock items that apply their status effects and are immediately destroyed without affecting your character's inventory or sub supplies.

**Q:** *Is it compatible with Neurotrauma and custom medical mods?*

- **A:** Actually not. The simulator hooks directly into Barotrauma's native `CharacterHealth` engine but doesn't fully supports all custom treats that Neurotrauma, but will be able to compatible a future. Other custom medical mods, probably yes.

## License & Copyright

This project is licensed under the **GNU General Public License v3.0 (GPLv3)**.  
See the [LICENSE](LICENSE) file in the project root for the full text of the license.

### Key Terms

- **Freedom to Use and Modify** — You may use, study, modify, and run this software for any purpose, both privately and publicly.
- **Attribution Required** — Any redistribution or publication of this project or its derivatives must retain the original copyright notice and clearly credit the author (**[@Retype15](https://github.com/Retype15)**).
- **Copyleft Protection** — Any modified version that is distributed must also be licensed under **GPLv3**, ensuring that all derivatives remain free and open.
- **Source Availability** — If you distribute a modified version, you must also provide access to the corresponding source code under the same license terms.

---

*Github Project: [SOS-DummyManager](https://github.com/Retype15/SOS-DummyManager)*  
*Developed by [@Retype15](https://github.com/Retype15)*

[![Subscribe!](https://img.shields.io/badge/Subscribe-2EA043?style=for-the-badge&logo=steam&logoColor=white)](https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282)  [![Add to Favorites](https://img.shields.io/badge/Add%20to%20Favorites-F0C040?style=for-the-badge&logo=steam&logoColor=white)](https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282)  [![Rate with a upvote!](https://img.shields.io/badge/Rate%20+1-3B82F6?style=for-the-badge&logo=steam&logoColor=white)](https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282)

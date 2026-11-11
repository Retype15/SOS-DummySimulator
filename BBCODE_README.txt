[h1]S.O.S. - Dummy Simulator[/h1]

[hr][/hr]

[b]S.O.S. - Dummy Simulator[/b] is the clinical training and medical simulation satellite plugin for [b]S.O.S. (Standard Operations Schematics)[/b] in [b]Barotrauma[/b]. Designed for doctors, combat medics, and modders, it integrates a fully interactive test patient directly into your S.O.S. inspector window—allowing you to experiment with lethal afflictions, test experimental treatments, and observe disease progression in real time without wasting precious medical supplies or endangering your crew.

[h2]Key Features[/h2]

[list]
[*] [b]Interactive Clinical Simulator:[/b] Seamlessly embeds Barotrauma's native medical inspection interface inside S.O.S. whenever an Item or Affliction is selected.
[*] [b]Zero-Risk Medical Testing:[/b] Safely diagnose conditions, inject modded toxins, test surgical procedures, and practice treatments without consuming actual inventory items.
[*] [b]Precision Affliction Control:[/b] Inject any affliction onto specific limbs or the whole body, fine-tune strength with live sliders, instantly set maximum strength (100%), or cure conditions on demand.
[*] [b]Mock Item Application:[/b] Simulate using any medical item or weapon on individual limbs to observe immediate biological effects and degree of success.
[*] [b]Time Manipulation:[/b] Control biological simulation time with Play/Pause and variable time acceleration ([b]x1, x2, x5, x10[/b]) to observe how afflictions worsen, stabilize, or heal naturally over time.
[*] [b]Interactive Body Navigation:[/b] Click on any active affliction on the patient to inspect its full S.O.S. documentation, or right-click to modify its intensity on the fly.
[*] [b]Mortality Tracker:[/b] Tracks cumulative dummy casualties across your campaign with persistent patient state.
[/list]

[h2]Requirements[/h2]

This mod is an official extension and requires the following base mods installed and enabled:

[olist]
[*] [url=https://github.com/evilfactory/LuaCsForBarotrauma][b]LuaCsForBarotrauma[/b][/url] (And enable C# Scripting).
[*] [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282][b]S.O.S. - Standard Operations Schematics[/b][/url] (Core).
[/olist]

[h2]Controls & Usage[/h2]

[h3]Inside S.O.S[/h3]

[olist]
[*] Open [b]S.O.S.[/b] (default hotkey: [b][J][/b]).
[*] Select any [b]Item[/b] or [b]Affliction[/b] in the browser.
[*] Switch to the [b]Simulator[/b] tab in the central panel.
[/olist]

[h3]Simulation Interactions[/h3]

[list]
[*] [b][Right Click on a Limb]:[/b] Opens the simulation context menu for the targeted limb, allowing you to inject the currently selected affliction or apply the selected medical item.
[*] [b][Left Click on an Affliction Icon]:[/b] Navigates S.O.S. directly to that affliction's documentation page.
[*] [b][Right Click on an Affliction Icon]:[/b] Opens the strength slider menu to increase, reduce, or remove that specific affliction.
[*] [b][START / PAUSE]:[/b] Freezes or resumes biological updates and affliction progression.
[*] [b][Speed Dropdown]:[/b] Accelerates simulation time (x1, x2, x5, x10) to observe long-term affliction effects.
[*] [b][RESET]:[/b] Reverts the patient's health and afflictions to the snapshot taken before the simulation was started.
[*] [b][DROP]:[/b] Discards the current patient, records a casualty in your statistics, and spawns a fresh, healthy dummy subject.
[/list]

[h2]Common questions[/h2]

[b]Q:[/b] [i]Can it be used on multiplayer and vanilla servers?[/i]
[list]
[*] [b]A:[/b] Yes! Just like S.O.S., this is a client-side utility mod. It runs entirely on your client and does not require the server or other players to have it installed.
[/list]

[b]Q:[/b] [i]Does simulating items consume items from my inventory?[/i]
[list]
[*] [b]A:[/b] Absolutely not. The simulator spawns temporary mock items that apply their status effects and are immediately destroyed without affecting your character's inventory or sub supplies.
[/list]

[b]Q:[/b] [i]Is it compatible with Neurotrauma and custom medical mods?[/i]
[list]
[*] [b]A:[/b] Actually not. The simulator hooks directly into Barotrauma's native [code]CharacterHealth[/code] engine but doesn't fully supports all custom treats that Neurotrauma, but will be able to compatible a future. Other custom medical mods, probably yes.
[/list]

[h2]License & Copyright[/h2]

This project is licensed under the [b]GNU General Public License v3.0 (GPLv3)[/b].  
See the LICENSE file in the project root for the full text of the license.

[h3]Key Terms[/h3]

[list]
[*] [b]Freedom to Use and Modify[/b] — You may use, study, modify, and run this software for any purpose, both privately and publicly.
[*] [b]Attribution Required[/b] — Any redistribution or publication of this project or its derivatives must retain the original copyright notice and clearly credit the author ([url=https://github.com/Retype15][b]@Retype15[/b][/url]).
[*] [b]Copyleft Protection[/b] — Any modified version that is distributed must also be licensed under [b]GPLv3[/b], ensuring that all derivatives remain free and open.
[*] [b]Source Availability[/b] — If you distribute a modified version, you must also provide access to the corresponding source code under the same license terms.
[/list]

[hr][/hr]

[i]Github Project: [url=https://github.com/Retype15/SOS-DummyManager]SOS-DummyManager[/url][/i]  
[i]Developed by [url=https://github.com/Retype15]@Retype15[/url][/i]

[url=https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282][img]https://img.shields.io/badge/Subscribe-2EA043?style=for-the-badge&logo=steam&logoColor=white[/img][/url]  [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282][img]https://img.shields.io/badge/Add%20to%20Favorites-F0C040?style=for-the-badge&logo=steam&logoColor=white[/img][/url]  [url=https://steamcommunity.com/sharedfiles/filedetails/?id=3682891282][img]https://img.shields.io/badge/Rate%20+1-3B82F6?style=for-the-badge&logo=steam&logoColor=white[/img][/url]
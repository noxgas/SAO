# Sword Art Online: Aincrad

A browser-based RPG game inspired by the anime *Sword Art Online*, built with vanilla HTML, CSS and JavaScript — no dependencies, no build step.

## How to Play

Open `index.html` in any modern web browser. No server required.

## Features

| Feature | Description |
|---|---|
| **100-Floor Dungeon** | Explore all 10 zones of Aincrad, each with unique enemies and a floor boss |
| **3 Playable Classes** | Swordsman (balanced), Knight (tank), Berserker (glass cannon) |
| **Sword Skills** | Each class has unique special skills; more unlock as you level up |
| **Turn-Based Combat** | Attack, use Sword Skills, consume items, or flee (not from bosses!) |
| **Boss Battles** | Named floor bosses with special skills, guarding each zone |
| **Permanent Death** | Losing all HP ends the run — just like in the anime |
| **Equipment & Shop** | Buy and equip weapons/armor; better gear unlocks on higher floors |
| **Inventory** | Collect potions and items; use them in or out of combat |
| **Level System** | Gain EXP from battles; level up to raise stats and unlock skills |
| **SAO-Inspired UI** | Dark blue aesthetic matching the anime's iconic interface |

## Game Structure

```
Floor 1–10   Plains of Beginning   Boss: Illfang the Kobold Lord
Floor 11–20  Foggy Forest          Boss: Asterios the Taurus King
Floor 21–30  Twilight Marsh        Boss: Kagachi the Undead Warlord
Floor 31–40  Granite Caverns       Boss: Golem of the Deep Forge
Floor 41–50  Volcanic Highlands    Boss: Baran the Dragon Monarch
Floor 51–60  Frozen Tundra         Boss: Hecate the Frost Queen
Floor 61–70  Ancient Ruins         Boss: Leviathan the Ruin God
Floor 71–80  Abyssal Depths        Boss: The Abyss Incarnate
Floor 81–90  Sky Realm             Boss: Aether the Sky God
Floor 91–100 Aincrad's Peak        Boss: Heathcliff — The Paladin (Final Boss)
```

## Controls

All interaction is via mouse/touch — click buttons to act. No keyboard shortcuts required.

## Files

```
index.html      Main game page
css/style.css   SAO-inspired dark UI
js/data.js      All static game data (enemies, skills, items, bosses)
js/game.js      Game engine, combat system, UI management
```

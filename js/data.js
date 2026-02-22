/* ═══════════════════════════════════════════════════
   SWORD ART ONLINE: Aincrad — Game Data
   All static game content: classes, skills, zones,
   enemies, bosses, weapons, armors, consumables.
   ═══════════════════════════════════════════════════ */

'use strict';

const DATA = {

    /* ── CLASSES ── */
    classes: {
        swordsman: {
            name: 'Swordsman',
            icon: '⚔️',
            baseStats: { hp: 100, maxHp: 100, sp: 25, maxSp: 25, atk: 13, def: 4, agi: 13 },
            levelGains: { hp: 9,  sp: 2, atk: 2, def: 1, agi: 1 },
            skills: ['horizontal_arc', 'vertical_square', 'sonic_leap']
        },
        knight: {
            name: 'Knight',
            icon: '🛡️',
            baseStats: { hp: 145, maxHp: 145, sp: 20, maxSp: 20, atk: 11, def: 9, agi: 8 },
            levelGains: { hp: 15, sp: 1, atk: 1, def: 2, agi: 0 },
            skills: ['shield_bash', 'iron_stance', 'heavy_strike']
        },
        berserker: {
            name: 'Berserker',
            icon: '🗡️',
            baseStats: { hp: 90, maxHp: 90, sp: 25, maxSp: 25, atk: 17, def: 2, agi: 12 },
            levelGains: { hp: 7,  sp: 2, atk: 3, def: 0, agi: 1 },
            skills: ['dual_fang', 'vorpal_strike', 'berserk_mode']
        }
    },

    /* ── SWORD SKILLS ──
       type: 'damage' | 'damage_pierce' | 'damage_stun' | 'buff' | 'buff_debuff'
    */
    skills: {
        /* Swordsman */
        horizontal_arc: {
            name: 'Horizontal Arc',
            icon: '⚔️',
            desc: 'A swift horizontal sweep.',
            type: 'damage',
            multiplier: 1.5,
            hits: 1,
            spCost: 4,
            cooldown: 0
        },
        vertical_square: {
            name: 'Vertical Square',
            icon: '✦',
            desc: 'Four rapid vertical slashes.',
            type: 'damage',
            multiplier: 0.75,
            hits: 4,
            spCost: 10,
            cooldown: 1
        },
        sonic_leap: {
            name: 'Sonic Leap',
            icon: '💨',
            desc: 'A leaping strike with crushing force.',
            type: 'damage',
            multiplier: 2.3,
            hits: 1,
            spCost: 8,
            cooldown: 1
        },
        star_splash: {
            name: 'Star Splash',
            icon: '✦',
            desc: 'Six consecutive star-pattern stabs.',
            type: 'damage',
            multiplier: 0.55,
            hits: 6,
            spCost: 14,
            cooldown: 2,
            unlockLevel: 12
        },
        /* Knight */
        shield_bash: {
            name: 'Shield Bash',
            icon: '🛡️',
            desc: 'Bash with shield, chance to stun.',
            type: 'damage_stun',
            multiplier: 0.85,
            hits: 1,
            spCost: 5,
            cooldown: 2,
            stunChance: 0.35
        },
        iron_stance: {
            name: 'Iron Stance',
            icon: '🔒',
            desc: 'Defensive stance, greatly raises DEF for 3 turns.',
            type: 'buff',
            stat: 'def',
            buffAmount: 10,
            buffDuration: 3,
            spCost: 6,
            cooldown: 4
        },
        heavy_strike: {
            name: 'Heavy Strike',
            icon: '⚒️',
            desc: 'A slow but devastating blow.',
            type: 'damage',
            multiplier: 3.0,
            hits: 1,
            spCost: 12,
            cooldown: 2
        },
        holy_blade: {
            name: 'Holy Blade',
            icon: '✝️',
            desc: 'A blessed strike that ignores half defense.',
            type: 'damage_pierce',
            multiplier: 2.5,
            hits: 1,
            spCost: 14,
            cooldown: 2,
            pierce: 0.5,
            unlockLevel: 14
        },
        /* Berserker */
        dual_fang: {
            name: 'Dual Fang',
            icon: '🗡️',
            desc: 'Two rapid slashes in quick succession.',
            type: 'damage',
            multiplier: 0.9,
            hits: 2,
            spCost: 5,
            cooldown: 0
        },
        vorpal_strike: {
            name: 'Vorpal Strike',
            icon: '💥',
            desc: 'A devastating blow, ignores 50% of DEF.',
            type: 'damage_pierce',
            multiplier: 3.2,
            hits: 1,
            spCost: 12,
            cooldown: 2,
            pierce: 0.5
        },
        berserk_mode: {
            name: 'Berserk Mode',
            icon: '🔥',
            desc: 'ATK +8, DEF −4 for 3 turns.',
            type: 'buff_debuff',
            atkBuff: 8,
            defDebuff: -4,
            duration: 3,
            spCost: 6,
            cooldown: 5
        },
        eclipse: {
            name: 'Eclipse',
            icon: '🌑',
            desc: 'Three shadow-infused strikes.',
            type: 'damage',
            multiplier: 0.9,
            hits: 3,
            spCost: 10,
            cooldown: 1,
            unlockLevel: 12
        }
    },

    /* ── ZONES (10 zones, 10 floors each) ── */
    zones: [
        { name: 'Plains of Beginning',  floorMin: 1,  floorMax: 10,  desc: 'Rolling fields with weak animals and goblins.' },
        { name: 'Foggy Forest',          floorMin: 11, floorMax: 20,  desc: 'Dense fog conceals ancient predators and undead.' },
        { name: 'Twilight Marsh',        floorMin: 21, floorMax: 30,  desc: 'Damp marshlands haunted by restless spirits.' },
        { name: 'Granite Caverns',       floorMin: 31, floorMax: 40,  desc: 'Underground tunnels patrolled by stone creatures.' },
        { name: 'Volcanic Highlands',    floorMin: 41, floorMax: 50,  desc: 'Scorched earth ruled by fire drakes and lava titans.' },
        { name: 'Frozen Tundra',         floorMin: 51, floorMax: 60,  desc: 'A vast frozen wasteland with howling blizzards.' },
        { name: 'Ancient Ruins',         floorMin: 61, floorMax: 70,  desc: 'Crumbling civilisation guarded by dark knights.' },
        { name: 'Abyssal Depths',        floorMin: 71, floorMax: 80,  desc: 'Absolute darkness where shadow beings dwell.' },
        { name: 'Sky Realm',             floorMin: 81, floorMax: 90,  desc: 'The upper spires of Aincrad, home to sky dragons.' },
        { name: "Aincrad's Peak",        floorMin: 91, floorMax: 100, desc: 'The final fortress. The truth lies at the top.' }
    ],

    /* ── TOWN NAMES ── */
    townNames: [
        'Town of Beginnings', 'Forest Haven', 'Marshland Outpost', "Miner's Rest",
        'Ember Town', 'Frost Citadel', 'Ruins Sanctuary', 'Abyss Gate',
        'Sky Pier', 'Aincrad Spire'
    ],

    /* ── ENEMY TEMPLATES ── (stats scaled per floor in game.js) */
    enemies: [
        /* Zone 0 – Plains */
        { id: 'boar',        name: 'Frenzy Boar',       sprite: '🐗', zone: 0, hp: 1.0, atk: 1.0, def: 0.8, exp: 20,  col: 14  },
        { id: 'goblin',      name: 'Goblin Raider',      sprite: '👺', zone: 0, hp: 0.8, atk: 0.9, def: 0.8, exp: 18,  col: 12  },
        { id: 'wolf',        name: 'Forest Wolf',        sprite: '🐺', zone: 0, hp: 0.9, atk: 1.1, def: 0.7, exp: 22,  col: 16  },
        { id: 'slime',       name: 'Dark Slime',         sprite: '🫧', zone: 0, hp: 1.2, atk: 0.7, def: 0.5, exp: 15,  col: 10  },
        /* Zone 1 – Foggy Forest */
        { id: 'lizardman',   name: 'Lizardman',          sprite: '🦎', zone: 1, hp: 1.1, atk: 1.0, def: 1.1, exp: 40,  col: 30  },
        { id: 'giant_bat',   name: 'Giant Bat',          sprite: '🦇', zone: 1, hp: 0.7, atk: 1.3, def: 0.6, exp: 35,  col: 26  },
        { id: 'skeleton',    name: 'Skeleton Soldier',   sprite: '💀', zone: 1, hp: 0.9, atk: 1.0, def: 1.0, exp: 38,  col: 28  },
        { id: 'treant',      name: 'Treant',             sprite: '🌳', zone: 1, hp: 1.5, atk: 0.8, def: 1.3, exp: 45,  col: 34  },
        /* Zone 2 – Twilight Marsh */
        { id: 'swamp_troll', name: 'Swamp Troll',        sprite: '👹', zone: 2, hp: 1.3, atk: 1.1, def: 0.9, exp: 65,  col: 50  },
        { id: 'marsh_witch', name: 'Marsh Witch',        sprite: '🧙', zone: 2, hp: 0.8, atk: 1.4, def: 0.7, exp: 60,  col: 54  },
        { id: 'zombie_knight','name': 'Zombie Knight',   sprite: '🧟', zone: 2, hp: 1.2, atk: 1.0, def: 1.2, exp: 70,  col: 48  },
        { id: 'water_serpent','name': 'Water Serpent',   sprite: '🐍', zone: 2, hp: 1.0, atk: 1.2, def: 0.8, exp: 62,  col: 52  },
        /* Zone 3 – Granite Caverns */
        { id: 'stone_golem', name: 'Stone Golem',        sprite: '🗿', zone: 3, hp: 1.6, atk: 1.0, def: 1.5, exp: 90,  col: 70  },
        { id: 'cave_spider', name: 'Cave Spider',        sprite: '🕷️', zone: 3, hp: 0.9, atk: 1.3, def: 0.8, exp: 85,  col: 64  },
        { id: 'dark_dwarf',  name: 'Dark Dwarf',         sprite: '⛏️', zone: 3, hp: 1.1, atk: 1.2, def: 1.2, exp: 92,  col: 74  },
        { id: 'rock_crab',   name: 'Rock Crab',          sprite: '🦀', zone: 3, hp: 1.4, atk: 0.9, def: 1.6, exp: 88,  col: 68  },
        /* Zone 4 – Volcanic Highlands */
        { id: 'fire_elem',   name: 'Fire Elemental',     sprite: '🔥', zone: 4, hp: 1.0, atk: 1.5, def: 0.8, exp: 120, col: 94  },
        { id: 'dragon_whelp','name': 'Dragon Whelp',     sprite: '🐉', zone: 4, hp: 1.2, atk: 1.3, def: 1.0, exp: 130, col: 104 },
        { id: 'lava_titan',  name: 'Lava Titan',         sprite: '🌋', zone: 4, hp: 1.8, atk: 1.1, def: 1.2, exp: 135, col: 100 },
        { id: 'phoenix_imp', name: 'Phoenix Imp',        sprite: '🦅', zone: 4, hp: 0.9, atk: 1.4, def: 0.9, exp: 124, col: 98  },
        /* Zone 5 – Frozen Tundra */
        { id: 'frost_wolf',  name: 'Frost Wolf',         sprite: '🐺', zone: 5, hp: 1.0, atk: 1.3, def: 1.0, exp: 155, col: 124 },
        { id: 'ice_golem',   name: 'Ice Golem',          sprite: '🧊', zone: 5, hp: 1.7, atk: 1.0, def: 1.6, exp: 165, col: 130 },
        { id: 'snow_banshee','name': 'Snow Banshee',     sprite: '👻', zone: 5, hp: 0.8, atk: 1.6, def: 0.7, exp: 158, col: 128 },
        { id: 'tundra_bear', name: 'Tundra Bear',        sprite: '🐻', zone: 5, hp: 1.5, atk: 1.2, def: 1.1, exp: 160, col: 132 },
        /* Zone 6 – Ancient Ruins */
        { id: 'dark_knight', name: 'Dark Knight',        sprite: '⚔️', zone: 6, hp: 1.2, atk: 1.3, def: 1.3, exp: 200, col: 164 },
        { id: 'stone_sent',  name: 'Stone Sentinel',     sprite: '🏛️', zone: 6, hp: 2.0, atk: 1.0, def: 1.8, exp: 210, col: 170 },
        { id: 'cursed_mage', name: 'Cursed Mage',        sprite: '🧙', zone: 6, hp: 0.9, atk: 1.7, def: 0.8, exp: 195, col: 158 },
        { id: 'spec_knight', name: 'Spectral Knight',    sprite: '👻', zone: 6, hp: 1.1, atk: 1.4, def: 1.1, exp: 205, col: 168 },
        /* Zone 7 – Abyssal Depths */
        { id: 'shadow_beast','name': 'Shadow Beast',     sprite: '🌑', zone: 7, hp: 1.3, atk: 1.5, def: 1.0, exp: 260, col: 210 },
        { id: 'void_crawler','name': 'Void Crawler',     sprite: '🕷️', zone: 7, hp: 1.1, atk: 1.6, def: 0.9, exp: 254, col: 204 },
        { id: 'abyss_demon', name: 'Abyss Demon',        sprite: '👿', zone: 7, hp: 1.4, atk: 1.4, def: 1.1, exp: 270, col: 220 },
        { id: 'dark_serpent','name': 'Dark Serpent',     sprite: '🐍', zone: 7, hp: 1.2, atk: 1.5, def: 1.0, exp: 265, col: 215 },
        /* Zone 8 – Sky Realm */
        { id: 'sky_dragon',  name: 'Sky Dragon',         sprite: '🐲', zone: 8, hp: 1.5, atk: 1.5, def: 1.2, exp: 340, col: 274 },
        { id: 'storm_hawk',  name: 'Storm Hawk',         sprite: '🦅', zone: 8, hp: 0.9, atk: 1.8, def: 0.8, exp: 330, col: 264 },
        { id: 'cloud_giant', name: 'Cloud Giant',        sprite: '☁️', zone: 8, hp: 2.2, atk: 1.3, def: 1.4, exp: 355, col: 284 },
        { id: 'cel_guard',   name: 'Celestial Guardian', sprite: '⚡', zone: 8, hp: 1.4, atk: 1.6, def: 1.3, exp: 345, col: 280 },
        /* Zone 9 – Aincrad's Peak */
        { id: 'ainc_knight', name: 'Aincrad Knight',     sprite: '⚔️', zone: 9, hp: 1.6, atk: 1.7, def: 1.5, exp: 450, col: 364 },
        { id: 'chaos_elem',  name: 'Chaos Elemental',    sprite: '🌀', zone: 9, hp: 1.4, atk: 1.9, def: 1.2, exp: 440, col: 354 },
        { id: 'elder_dragon','name': 'Elder Dragon',     sprite: '🐉', zone: 9, hp: 2.5, atk: 1.6, def: 1.6, exp: 480, col: 390 },
        { id: 'void_emperor','name': 'Void Emperor',     sprite: '👑', zone: 9, hp: 1.8, atk: 2.0, def: 1.4, exp: 470, col: 380 }
    ],

    /* ── BOSSES (one per 10-floor zone) ── */
    bosses: [
        {
            floor: 10,
            name: 'Illfang the Kobold Lord',
            sprite: '👹',
            hp: 400, atk: 26, def: 8, exp: 500, col: 400,
            desc: 'Guardian of Floor 1. A massive kobold wielding a curved talwar.',
            skills: ['Howling Charge', 'Tail Strike']
        },
        {
            floor: 20,
            name: 'Asterios the Taurus King',
            sprite: '🐂',
            hp: 850, atk: 48, def: 16, exp: 1000, col: 800,
            desc: 'A bull-headed monstrosity with a club as tall as a tree.',
            skills: ['Gore Rush', 'Earthquake Stomp']
        },
        {
            floor: 30,
            name: 'Kagachi the Undead Warlord',
            sprite: '💀',
            hp: 1500, atk: 78, def: 24, exp: 1600, col: 1300,
            desc: 'An undead warlord who refuses to stay dead.',
            skills: ['Soul Drain', 'Bone Shatter']
        },
        {
            floor: 40,
            name: 'Golem of the Deep Forge',
            sprite: '🗿',
            hp: 2400, atk: 115, def: 38, exp: 2500, col: 2000,
            desc: 'An ancient stone giant forged in the heart of Aincrad.',
            skills: ['Rock Smash', 'Stone Skin']
        },
        {
            floor: 50,
            name: 'Baran the Dragon Monarch',
            sprite: '🐉',
            hp: 3400, atk: 160, def: 50, exp: 3600, col: 3000,
            desc: 'A fire-breathing dragon who rules the volcanic highlands.',
            skills: ['Flame Breath', 'Wing Buffet']
        },
        {
            floor: 60,
            name: 'Hecate the Frost Queen',
            sprite: '❄️',
            hp: 4800, atk: 220, def: 65, exp: 5200, col: 4200,
            desc: 'An ice sorceress whose blizzards can freeze time itself.',
            skills: ['Blizzard', 'Ice Lance Barrage']
        },
        {
            floor: 70,
            name: 'Leviathan the Ruin God',
            sprite: '🌊',
            hp: 6400, atk: 295, def: 84, exp: 6800, col: 5600,
            desc: 'The ancient god of ruins, awakened from eternal slumber.',
            skills: ['Tidal Wave', 'Ruin Pulse']
        },
        {
            floor: 80,
            name: 'The Abyss Incarnate',
            sprite: '🌑',
            hp: 8500, atk: 380, def: 106, exp: 9000, col: 7500,
            desc: 'A living manifestation of the void between worlds.',
            skills: ['Void Crush', 'Darkness Nova']
        },
        {
            floor: 90,
            name: 'Aether the Sky God',
            sprite: '⚡',
            hp: 11000, atk: 480, def: 130, exp: 11500, col: 9500,
            desc: 'The celestial ruler of the sky realm, wielding pure lightning.',
            skills: ['Thunder God Wrath', 'Storm Lance']
        },
        {
            floor: 100,
            name: 'Heathcliff — The Paladin',
            sprite: '⚔️',
            hp: 15000, atk: 620, def: 165, exp: 20000, col: 15000,
            desc: 'The Game Master himself. Kayaba Akihiko in the flesh. Defeat him to free everyone.',
            skills: ['Holy Sword', 'Divine Judgement', 'System Override'],
            isFinalBoss: true
        }
    ],

    /* ── WEAPONS ── */
    weapons: [
        { id: 'bronze_sword',    name: 'Bronze Sword',      icon: '⚔️',  slot: 'weapon', atk: 3,   agi: 0, def: 0,  minFloor: 0,  cost: 0,     desc: 'A basic bronze sword.' },
        { id: 'iron_sword',      name: 'Iron Sword',        icon: '⚔️',  slot: 'weapon', atk: 9,   agi: 0, def: 0,  minFloor: 1,  cost: 300,   desc: 'A dependable iron sword.' },
        { id: 'steel_blade',     name: 'Steel Blade',       icon: '⚔️',  slot: 'weapon', atk: 16,  agi: 1, def: 0,  minFloor: 6,  cost: 750,   desc: 'A well-forged steel blade.' },
        { id: 'anneal_blade',    name: 'Anneal Blade',      icon: '⚔️',  slot: 'weapon', atk: 24,  agi: 2, def: 0,  minFloor: 11, cost: 1600,  desc: 'A special anneal blade from the first boss.' },
        { id: 'dark_blade',      name: 'Dark Blade',        icon: '🗡️',  slot: 'weapon', atk: 35,  agi: 2, def: 0,  minFloor: 21, cost: 3200,  desc: 'A blade forged in eternal darkness.' },
        { id: 'serpent_fang',    name: 'Serpent Fang',      icon: '🗡️',  slot: 'weapon', atk: 48,  agi: 3, def: 0,  minFloor: 31, cost: 5500,  desc: 'A curved blade made from a serpent\'s fang.' },
        { id: 'elucidator',      name: 'Elucidator',        icon: '⚫',  slot: 'weapon', atk: 64,  agi: 3, def: 0,  minFloor: 41, cost: 8500,  desc: 'A legendary black sword of unmatched sharpness.' },
        { id: 'dark_repulser',   name: 'Dark Repulser',     icon: '💙',  slot: 'weapon', atk: 82,  agi: 4, def: 0,  minFloor: 56, cost: 14000, desc: 'A crystalline blue sword radiating cold power.' },
        { id: 'celestial_sword', name: 'Celestial Sword',   icon: '✨',  slot: 'weapon', atk: 105, agi: 5, def: 0,  minFloor: 71, cost: 22000, desc: 'A sword infused with celestial energy.' },
        { id: 'aincrad_sword',   name: "Aincrad's Edge",    icon: '⚡',  slot: 'weapon', atk: 140, agi: 6, def: 0,  minFloor: 91, cost: 35000, desc: 'The ultimate sword, forged at the peak of Aincrad.' }
    ],

    /* ── ARMORS ── */
    armors: [
        { id: 'cloth_armor',     name: 'Cloth Armor',       icon: '👕',  slot: 'armor',  def: 3,  agi: 0,  atk: 0, minFloor: 0,  cost: 0,     desc: 'Basic cloth protection.' },
        { id: 'leather_armor',   name: 'Leather Armor',     icon: '🥋',  slot: 'armor',  def: 8,  agi: 1,  atk: 0, minFloor: 1,  cost: 260,   desc: 'Leather armor for adventurers.' },
        { id: 'chainmail',       name: 'Chainmail',         icon: '🔗',  slot: 'armor',  def: 15, agi: 0,  atk: 0, minFloor: 6,  cost: 650,   desc: 'Interlocked metal rings.' },
        { id: 'plate_armor',     name: 'Plate Armor',       icon: '🛡️',  slot: 'armor',  def: 24, agi: -1, atk: 0, minFloor: 11, cost: 1300,  desc: 'Heavy full-plate protection.' },
        { id: 'dragon_scale',    name: 'Dragon Scale Mail', icon: '🐉',  slot: 'armor',  def: 38, agi: 0,  atk: 0, minFloor: 31, cost: 4200,  desc: 'Armor forged from dragon scales.' },
        { id: 'shadow_mail',     name: 'Shadow Mail',       icon: '🌑',  slot: 'armor',  def: 54, agi: 2,  atk: 1, minFloor: 51, cost: 9000,  desc: 'Armor tempered in abyssal shadow.' },
        { id: 'celestial_armor', name: 'Celestial Armor',   icon: '✨',  slot: 'armor',  def: 76, agi: 2,  atk: 0, minFloor: 71, cost: 17000, desc: 'Divine armor of celestial origin.' },
        { id: 'aincrad_armor',   name: 'Aincrad Royal Mail',icon: '⚡',  slot: 'armor',  def: 108,agi: 1,  atk: 0, minFloor: 91, cost: 28000, desc: 'The ultimate armor of Aincrad.' }
    ],

    /* ── CONSUMABLES ── */
    consumables: [
        { id: 'small_potion',  name: 'Health Potion',   icon: '🧪', effect: 'heal',      value: 60,   cost: 80,   desc: 'Restores 60 HP.' },
        { id: 'med_potion',    name: 'Hi-Potion',       icon: '💊', effect: 'heal',      value: 200,  cost: 220,  desc: 'Restores 200 HP.' },
        { id: 'large_potion',  name: 'Max Potion',      icon: '💉', effect: 'heal',      value: 600,  cost: 700,  desc: 'Restores 600 HP.' },
        { id: 'mega_crystal',  name: 'Life Crystal',    icon: '💎', effect: 'heal',      value: 2500, cost: 2200, desc: 'Restores 2500 HP.' },
        { id: 'sp_crystal',    name: 'SP Crystal',      icon: '🔷', effect: 'sp',        value: 15,   cost: 160,  desc: 'Restores 15 SP.' },
        { id: 'sp_gem',        name: 'SP Gem',          icon: '🔹', effect: 'sp',        value: 50,   cost: 500,  desc: 'Restores 50 SP.' },
        { id: 'antidote',      name: 'Antidote',        icon: '💚', effect: 'cure',      value: 'poison', cost: 100, desc: 'Cures Poison.' },
        { id: 'elixir',        name: 'Full Elixir',     icon: '🌟', effect: 'full_heal', value: 0,    cost: 3500, desc: 'Fully restores HP and SP.' }
    ]
};

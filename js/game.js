/* ═══════════════════════════════════════════════════
   SWORD ART ONLINE: Aincrad — Game Engine
   ═══════════════════════════════════════════════════ */

'use strict';

/* ──────────────────────────────────────────
   UTILITY HELPERS
   ────────────────────────────────────────── */
function rand(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}
function clamp(v, lo, hi) {
    return Math.max(lo, Math.min(hi, v));
}
function el(id) {
    return document.getElementById(id);
}

/* ──────────────────────────────────────────
   GAME OBJECT — main state machine
   ────────────────────────────────────────── */
const game = {
    player: null,
    dungeonProgress: 0,   // 0–100; 80+ unlocks boss
    pendingRewards: null, // filled after a fight wins
    pendingLevelUps: [],  // queued level-up stat objects

    /* ── INIT ── */
    init() {
        this._bindEvents();
        this.showScreen('title');
    },

    /* ── SCREEN MANAGEMENT ── */
    showScreen(id) {
        document.querySelectorAll('.screen').forEach(s => {
            s.classList.remove('active');
            s.style.display = 'none';
        });
        const s = el('screen-' + id);
        if (s) { s.style.display = 'flex'; s.classList.add('active'); }
    },

    showPanel(id) {
        document.querySelectorAll('.panel').forEach(p => p.classList.add('hidden'));
        const p = el('panel-' + id);
        if (p) p.classList.remove('hidden');
        if (id === 'shop')   this._renderShop('weapons');
        if (id === 'status') this._renderStatus();
        if (id === 'dungeon') this._refreshDungeon();
        if (id === 'town')   this._refreshTown();
    },

    /* ── CHARACTER CREATION ── */
    _validateCreate() {
        const name = el('player-name').value.trim();
        const cls  = document.querySelector('.class-card.selected');
        el('btn-create').disabled = !(name.length && cls);
    },

    createCharacter() {
        const name = el('player-name').value.trim();
        const clsKey = document.querySelector('.class-card.selected').dataset.class;
        const cls  = DATA.classes[clsKey];

        this.player = {
            name,
            classKey: clsKey,
            className: cls.name,
            level: 1,
            exp: 0,
            expToNext: 100,
            col: 500,
            currentFloor: 1,
            equipment: {
                weapon: Object.assign({}, DATA.weapons[0]),
                armor:  Object.assign({}, DATA.armors[0])
            },
            inventory: [
                Object.assign({}, DATA.consumables[0], { qty: 3 }),
                Object.assign({}, DATA.consumables[4], { qty: 2 })
            ],
            skills: cls.skills.slice(),
            cooldowns: {},   // skillId -> turnsRemaining
            buffs:    [],    // { stat, amount, duration }
            statusEffects: [],
            levelGains: cls.levelGains,
            /* base stats copied in */
            ...JSON.parse(JSON.stringify(cls.baseStats))
        };

        this.dungeonProgress = 0;
        this._calcStats();
        this.player.hp = this.player.maxHp;
        this.player.sp = this.player.maxSp;

        this.showScreen('main');
        this._refreshTown();
        this.showPanel('town');
        this._updateHUD();
    },

    /* ── STAT CALCULATION ── */
    _calcStats() {
        const p = this.player;
        let atkBonus = 0, defBonus = 0, agiBonus = 0;

        if (p.equipment.weapon) {
            atkBonus += p.equipment.weapon.atk || 0;
            agiBonus += p.equipment.weapon.agi || 0;
        }
        if (p.equipment.armor) {
            defBonus += p.equipment.armor.def || 0;
            agiBonus += (p.equipment.armor.agi || 0);
            atkBonus += (p.equipment.armor.atk || 0);
        }

        let buffAtk = 0, buffDef = 0;
        p.buffs.forEach(b => {
            if (b.stat === 'atk') buffAtk += b.amount;
            if (b.stat === 'def') buffDef += b.amount;
        });

        p.totalAtk = p.atk + atkBonus + buffAtk;
        p.totalDef = Math.max(0, p.def + defBonus + buffDef);
        p.totalAgi = p.agi + agiBonus;
    },

    /* ── HUD UPDATE ── */
    _updateHUD() {
        const p = this.player;
        el('hud-floor').textContent = p.currentFloor;
        el('hud-name').textContent  = p.name;
        el('hud-level').textContent = 'Lv.' + p.level;
        el('hud-col').textContent   = p.col.toLocaleString() + ' Col';

        const hpPct = clamp((p.hp / p.maxHp) * 100, 0, 100);
        const spPct = clamp((p.sp / p.maxSp) * 100, 0, 100);

        el('hud-hp-bar').style.width = hpPct + '%';
        el('hud-hp-text').textContent = p.hp + '/' + p.maxHp;
        el('hud-sp-bar').style.width = spPct + '%';
        el('hud-sp-text').textContent = p.sp + '/' + p.maxSp;

        /* HP colour warns when low */
        const hpBar = el('hud-hp-bar');
        if (hpPct < 25)      hpBar.style.background = 'linear-gradient(90deg,#880000,#cc0000)';
        else if (hpPct < 50) hpBar.style.background = 'linear-gradient(90deg,#aa2200,#ff4400)';
        else                 hpBar.style.background = 'linear-gradient(90deg,#bb2200,#ff4500)';
    },

    /* ── TOWN ── */
    _refreshTown() {
        const p  = this.player;
        const zi = this._zoneIndex(p.currentFloor);
        const z  = DATA.zones[zi];

        el('town-name').textContent = DATA.townNames[zi] || 'Unknown Town';
        el('town-zone').textContent = z.name + ' — Floor ' + p.currentFloor;
        el('town-desc').textContent = 'A safe haven at Floor ' + p.currentFloor + '. ' + z.desc;

        const restCost = this._restCost();
        el('btn-rest').textContent = '💊 Rest (' + restCost + ' Col)';
    },

    _restCost() {
        return Math.floor(30 + this.player.currentFloor * 5);
    },

    rest() {
        const p    = this.player;
        const cost = this._restCost();
        if (p.col < cost) { this._notify('Not enough Col! Need ' + cost + ' Col.'); return; }
        if (p.hp >= p.maxHp && p.sp >= p.maxSp) { this._notify('Already at full HP and SP!'); return; }
        p.col -= cost;
        p.hp   = p.maxHp;
        p.sp   = p.maxSp;
        p.statusEffects = [];
        p.buffs = [];
        this._calcStats();
        this._updateHUD();
        this._notify('Rested fully! HP and SP restored.');
    },

    /* ── DUNGEON ── */
    _refreshDungeon() {
        const p  = this.player;
        const zi = this._zoneIndex(p.currentFloor);
        const z  = DATA.zones[zi];

        el('dungeon-floor').textContent = 'Floor ' + p.currentFloor + ' — Dungeon';
        el('dungeon-zone').textContent  = z.name;
        el('dungeon-desc').textContent  =
            'Explore the labyrinth to find the boss chamber. ' +
            'Progress: ' + this.dungeonProgress + '%';
        el('progress-text').textContent = this.dungeonProgress + '%';
        el('progress-bar').style.width  = this.dungeonProgress + '%';

        el('btn-boss').disabled = (this.dungeonProgress < 80);
    },

    explore() {
        const enemy = this._buildEnemy(this.player.currentFloor);
        combat.start(enemy);
        this.showPanel('combat');
    },

    challengeBoss() {
        const boss = this._buildBoss(this.player.currentFloor);
        if (!boss) { this._notify('No boss found for this floor.'); return; }
        combat.start(boss);
        this.showPanel('combat');
    },

    /* ── ENEMY FACTORY ── */
    _zoneIndex(floor) {
        return DATA.zones.findIndex(z => floor >= z.floorMin && floor <= z.floorMax);
    },

    _buildEnemy(floor) {
        const zi       = this._zoneIndex(floor);
        const pool     = DATA.enemies.filter(e => e.zone === zi);
        const tmpl     = pool[rand(0, pool.length - 1)];
        const scale    = 1 + (floor - 1) * 0.14;

        return {
            name:   tmpl.name,
            sprite: tmpl.sprite,
            maxHp:  Math.floor(42 * scale * tmpl.hp),
            hp:     Math.floor(42 * scale * tmpl.hp),
            atk:    Math.floor(9  * scale * tmpl.atk),
            def:    Math.floor(2  * scale * tmpl.def),
            exp:    Math.floor(tmpl.exp  * (1 + (floor - 1) * 0.08)),
            col:    Math.floor(tmpl.col  * (1 + (floor - 1) * 0.08)),
            isBoss: false,
            statusEffects: []
        };
    },

    _buildBoss(floor) {
        const bossFloor = Math.ceil(floor / 10) * 10;
        const tmpl = DATA.bosses.find(b => b.floor === bossFloor);
        if (!tmpl) return null;

        /* Scale slightly if fighting before exact boss floor */
        const factor = floor / bossFloor;
        return {
            name:        tmpl.name,
            sprite:      tmpl.sprite,
            desc:        tmpl.desc,
            maxHp:       Math.floor(tmpl.hp  * factor),
            hp:          Math.floor(tmpl.hp  * factor),
            atk:         Math.floor(tmpl.atk * factor),
            def:         Math.floor(tmpl.def * factor),
            exp:         tmpl.exp,
            col:         tmpl.col,
            isBoss:      true,
            isFinalBoss: !!tmpl.isFinalBoss,
            bossSkills:  tmpl.skills || [],
            statusEffects: []
        };
    },

    /* ── COMBAT CALLBACKS ── */
    onCombatEnd(won, enemy) {
        if (!won) {
            this._showGameOver();
            return;
        }

        /* Gain progress */
        if (!enemy.isBoss) {
            this.dungeonProgress = Math.min(100, this.dungeonProgress + rand(8, 18));
        }

        /* Build reward info */
        const p = this.player;
        let expGained = enemy.exp;
        let colGained = enemy.col;
        let drop = null;

        /* Random equipment drop (boss always drops something) */
        if (enemy.isBoss || Math.random() < 0.15) {
            drop = this._pickDrop(p.currentFloor);
            if (drop) this._addToInventory(Object.assign({}, drop, { qty: 1 }));
        }

        /* Store pending rewards so level-up chain works */
        this.pendingRewards = { expGained, colGained, drop, enemy };

        /* Show reward screen */
        this._showRewardScreen(expGained, colGained, drop, enemy.isBoss);
    },

    _showRewardScreen(exp, col, drop, isBoss) {
        let html = '';
        html += `<div class="reward-row"><span>EXP</span><span>+${exp.toLocaleString()}</span></div>`;
        html += `<div class="reward-row"><span>Col</span><span>+${col.toLocaleString()}</span></div>`;
        if (drop) {
            html += `<div class="reward-drop">📦 Item drop: ${drop.icon || ''} ${drop.name}</div>`;
        }
        if (isBoss) {
            html += `<div class="reward-drop" style="color:var(--accent-gold)">✦ Boss defeated!</div>`;
        }
        el('reward-content').innerHTML = html;
        this.showScreen('reward');
    },

    afterReward() {
        const { expGained, colGained, enemy } = this.pendingRewards;
        const p = this.player;

        p.col += colGained;
        p.exp += expGained;

        /* Tick down cooldowns and buffs */
        this._tickCooldowns();
        this._tickBuffs();
        this._calcStats();
        this._updateHUD();

        /* Check level-up(s) */
        this.pendingLevelUps = [];
        while (p.exp >= p.expToNext) {
            p.exp -= p.expToNext;
            p.level++;
            p.expToNext = Math.floor(p.expToNext * 1.35);

            const gains = p.levelGains;
            p.maxHp += gains.hp;
            p.hp     = Math.min(p.hp + gains.hp, p.maxHp);
            p.maxSp += gains.sp;
            p.sp     = Math.min(p.sp + gains.sp, p.maxSp);
            p.atk   += gains.atk;
            p.def   += gains.def;
            p.agi   += gains.agi;

            /* Unlock new skills at certain levels */
            this._checkSkillUnlocks();

            this.pendingLevelUps.push({
                level: p.level,
                hp:  '+' + gains.hp,
                sp:  '+' + gains.sp,
                atk: '+' + gains.atk,
                def: '+' + gains.def,
                agi: '+' + gains.agi
            });
        }

        this._calcStats();
        this._updateHUD();

        if (this.pendingLevelUps.length) {
            this._showLevelUp();
        } else if (enemy.isBoss) {
            this._showFloorClear(enemy);
        } else {
            this.showScreen('main');
            this.showPanel('dungeon');
            this._refreshDungeon();
        }
    },

    _showLevelUp() {
        const lu = this.pendingLevelUps.shift();
        let html = `<div class="levelup-row"><span>Level</span><span>→ ${lu.level}</span></div>`;
        html += `<div class="levelup-row"><span>Max HP</span><span>${lu.hp}</span></div>`;
        html += `<div class="levelup-row"><span>Max SP</span><span>${lu.sp}</span></div>`;
        html += `<div class="levelup-row"><span>ATK</span><span>${lu.atk}</span></div>`;
        html += `<div class="levelup-row"><span>DEF</span><span>${lu.def}</span></div>`;
        html += `<div class="levelup-row"><span>AGI</span><span>${lu.agi}</span></div>`;
        el('levelup-content').innerHTML = html;
        this.showScreen('levelup');
    },

    afterLevelUp() {
        const enemy = this.pendingRewards.enemy;
        if (this.pendingLevelUps.length) {
            this._showLevelUp();
        } else if (enemy.isBoss) {
            this._showFloorClear(enemy);
        } else {
            this.showScreen('main');
            this.showPanel('dungeon');
            this._refreshDungeon();
        }
    },

    _showFloorClear(boss) {
        const p = this.player;
        let html = '';
        html += `<div class="reward-row"><span>Floor</span><span>${p.currentFloor} Cleared!</span></div>`;
        html += `<div class="reward-row"><span>Boss</span><span>${boss.name}</span></div>`;
        html += `<div class="reward-row"><span>Player Level</span><span>${p.level}</span></div>`;
        el('floor-clear-content').innerHTML = html;

        if (boss.isFinalBoss) {
            this._showVictory();
        } else {
            this.showScreen('floor-clear');
        }
    },

    advanceFloor() {
        const p = this.player;
        p.currentFloor++;
        this.dungeonProgress = 0;
        this._calcStats();
        this._updateHUD();
        this._refreshTown();
        this.showScreen('main');
        this.showPanel('town');
        this._notify('Welcome to Floor ' + p.currentFloor + '!');
    },

    /* ── SKILL UNLOCKS ── */
    _checkSkillUnlocks() {
        const p = this.player;
        Object.entries(DATA.skills).forEach(([id, sk]) => {
            if (sk.unlockLevel && p.level >= sk.unlockLevel && !p.skills.includes(id)) {
                /* Only unlock skills belonging to the player's class */
                const classSkills = { swordsman: ['star_splash'], knight: ['holy_blade'], berserker: ['eclipse'] };
                if ((classSkills[p.classKey] || []).includes(id)) {
                    p.skills.push(id);
                    this._notify('New skill unlocked: ' + sk.name + '!');
                }
            }
        });
    },

    /* ── ITEM DROP ── */
    _pickDrop(floor) {
        const allItems = [...DATA.weapons, ...DATA.armors, ...DATA.consumables];
        const eligible = allItems.filter(it =>
            it.minFloor !== undefined ? it.minFloor <= floor : true
        );
        if (!eligible.length) return null;
        const item = eligible[rand(0, eligible.length - 1)];
        return item;
    },

    _addToInventory(item) {
        const p   = this.player;
        const existing = p.inventory.find(i => i.id === item.id);
        if (existing && item.effect) {
            existing.qty = (existing.qty || 1) + 1;
        } else {
            p.inventory.push(Object.assign({}, item, { qty: 1 }));
        }
    },

    /* ── BUFF / COOLDOWN TICKS ── */
    _tickCooldowns() {
        const p = this.player;
        Object.keys(p.cooldowns).forEach(id => {
            if (p.cooldowns[id] > 0) p.cooldowns[id]--;
        });
    },

    _tickBuffs() {
        const p = this.player;
        p.buffs = p.buffs.filter(b => {
            b.duration--;
            return b.duration > 0;
        });
        p.statusEffects = p.statusEffects.filter(e => {
            e.duration--;
            return e.duration > 0;
        });
    },

    /* ── GAME OVER ── */
    _showGameOver() {
        const p = this.player;
        el('gameover-stats').innerHTML =
            `<strong>${p.name}</strong> — ${p.className}<br>` +
            `Level ${p.level} &nbsp;|&nbsp; Floor ${p.currentFloor}<br>` +
            `${(p.exp + (p.level - 1) * 100).toLocaleString()} total EXP`;
        this.showScreen('gameover');
    },

    /* ── VICTORY ── */
    _showVictory() {
        const p = this.player;
        el('victory-stats').innerHTML =
            `<strong>${p.name}</strong> — ${p.className}<br>` +
            `Level ${p.level} &nbsp;|&nbsp; All 100 Floors Cleared<br>` +
            `${p.col.toLocaleString()} Col remaining`;
        this.showScreen('victory');
    },

    /* ── SHOP ── */
    _renderShop(tab) {
        el('shop-col-display').textContent = this.player.col.toLocaleString() + ' Col';

        document.querySelectorAll('.shop-tab').forEach(t =>
            t.classList.toggle('active', t.dataset.tab === tab)
        );

        const floor = this.player.currentFloor;
        let items = [];
        if (tab === 'weapons') items = DATA.weapons.filter(w => w.minFloor <= floor);
        else if (tab === 'armors') items = DATA.armors.filter(a => a.minFloor <= floor);
        else items = DATA.consumables;

        const container = el('shop-items');
        container.innerHTML = '';

        items.forEach(item => {
            const isEquipped =
                (this.player.equipment.weapon && this.player.equipment.weapon.id === item.id) ||
                (this.player.equipment.armor  && this.player.equipment.armor.id  === item.id);

            const div = document.createElement('div');
            div.className = 'shop-item';
            div.innerHTML = `
                <div class="shop-item-info">
                    <div class="shop-item-name">${item.icon || ''} ${item.name}</div>
                    <div class="shop-item-desc">${item.desc}${this._itemStatLine(item)}</div>
                </div>
                <div class="shop-item-right">
                    <span class="shop-item-price">${item.cost > 0 ? item.cost.toLocaleString() + ' Col' : 'FREE'}</span>
                    ${isEquipped
                        ? '<span class="equipped-tag">EQUIPPED</span>'
                        : `<button class="btn btn-buy" data-id="${item.id}">Buy</button>`
                    }
                </div>`;

            if (!isEquipped) {
                div.querySelector('.btn-buy').addEventListener('click', () => this._buyItem(item.id));
            }
            container.appendChild(div);
        });
    },

    _itemStatLine(item) {
        const parts = [];
        if (item.atk) parts.push('ATK +' + item.atk);
        if (item.def) parts.push('DEF +' + item.def);
        if (item.agi && item.agi !== 0) parts.push('AGI ' + (item.agi > 0 ? '+' : '') + item.agi);
        if (item.value && item.effect === 'heal') parts.push('Heal ' + item.value + ' HP');
        if (item.value && item.effect === 'sp')   parts.push('Restore ' + item.value + ' SP');
        return parts.length ? ' · ' + parts.join(' · ') : '';
    },

    _buyItem(itemId) {
        const all  = [...DATA.weapons, ...DATA.armors, ...DATA.consumables];
        const item = all.find(i => i.id === itemId);
        if (!item) return;

        const p = this.player;
        if (p.col < item.cost) { this._notify('Not enough Col!'); return; }

        p.col -= item.cost;

        /* Equip weapons/armor; add consumables to inventory */
        if (item.slot === 'weapon') {
            p.equipment.weapon = Object.assign({}, item);
            this._calcStats();
            this._notify('Equipped: ' + item.name);
        } else if (item.slot === 'armor') {
            p.equipment.armor = Object.assign({}, item);
            this._calcStats();
            this._notify('Equipped: ' + item.name);
        } else {
            this._addToInventory(item);
            this._notify('Bought: ' + item.name);
        }

        this._updateHUD();
        this._renderShop(document.querySelector('.shop-tab.active').dataset.tab);
    },

    /* ── STATUS PANEL ── */
    _renderStatus() {
        const p = this.player;
        this._calcStats();
        el('stat-name').textContent  = p.name;
        el('stat-class').textContent = p.className;
        el('stat-level').textContent = p.level;
        el('stat-exp').textContent   = p.exp + ' / ' + p.expToNext;
        el('stat-hp').textContent    = p.hp + ' / ' + p.maxHp;
        el('stat-sp').textContent    = p.sp + ' / ' + p.maxSp;
        el('stat-atk').textContent   = p.totalAtk + ' (base ' + p.atk + ')';
        el('stat-def').textContent   = p.totalDef + ' (base ' + p.def + ')';
        el('stat-agi').textContent   = p.totalAgi;
        el('stat-col').textContent   = p.col.toLocaleString() + ' Col';

        el('equip-weapon').textContent = p.equipment.weapon ? p.equipment.weapon.name : 'None';
        el('equip-armor').textContent  = p.equipment.armor  ? p.equipment.armor.name  : 'None';

        /* Inventory list */
        const inv = el('inventory-list');
        inv.innerHTML = '';
        if (!p.inventory.length) {
            inv.innerHTML = '<div style="color:var(--text-secondary);font-size:11px">Empty</div>';
            return;
        }
        p.inventory.forEach((item, idx) => {
            const d = document.createElement('div');
            d.className = 'inv-item';
            const canUse = item.effect && !combat.active;
            d.innerHTML = `
                <span class="inv-item-name">${item.icon || ''} ${item.name}</span>
                <span class="inv-item-qty">×${item.qty || 1}</span>
                ${canUse ? `<button class="btn inv-item-use" data-idx="${idx}">Use</button>` : ''}`;
            if (canUse) {
                d.querySelector('.inv-item-use').addEventListener('click', () => this._useItemOutOfCombat(idx));
            }
            inv.appendChild(d);
        });
    },

    _useItemOutOfCombat(idx) {
        const p    = this.player;
        const item = p.inventory[idx];
        if (!item) return;

        let msg = '';
        if (item.effect === 'heal') {
            const amt  = Math.min(item.value, p.maxHp - p.hp);
            p.hp += amt;
            msg = 'Used ' + item.name + '. Restored ' + amt + ' HP.';
        } else if (item.effect === 'sp') {
            const amt  = Math.min(item.value, p.maxSp - p.sp);
            p.sp += amt;
            msg = 'Used ' + item.name + '. Restored ' + amt + ' SP.';
        } else if (item.effect === 'full_heal') {
            p.hp = p.maxHp;
            p.sp = p.maxSp;
            msg = 'Used ' + item.name + '. Fully restored!';
        } else {
            msg = 'Cannot use that here.';
        }

        this._consumeItem(idx);
        this._updateHUD();
        this._renderStatus();
        this._notify(msg);
    },

    _consumeItem(idx) {
        const p    = this.player;
        const item = p.inventory[idx];
        if (!item) return;
        item.qty = (item.qty || 1) - 1;
        if (item.qty <= 0) p.inventory.splice(idx, 1);
    },

    /* ── COMBAT UI HELPERS ── */
    showSkillMenu() {
        el('combat-actions').style.display = 'none';
        el('item-menu').classList.add('hidden');
        el('skill-menu').classList.remove('hidden');
        this._renderSkillList();
    },

    showItemMenu() {
        el('combat-actions').style.display = 'none';
        el('skill-menu').classList.add('hidden');
        el('item-menu').classList.remove('hidden');
        this._renderCombatItems();
    },

    hideSubMenus() {
        el('skill-menu').classList.add('hidden');
        el('item-menu').classList.add('hidden');
        el('combat-actions').style.display = '';
    },

    _renderSkillList() {
        const p   = this.player;
        const container = el('skill-list');
        container.innerHTML = '';

        p.skills.forEach(id => {
            const sk  = DATA.skills[id];
            if (!sk) return;
            const cd  = p.cooldowns[id] || 0;
            const noSp = p.sp < sk.spCost;
            const disabled = cd > 0 || noSp;

            const d = document.createElement('div');
            d.className = 'skill-item' + (disabled ? ' disabled' : '');
            d.innerHTML = `
                <div class="skill-left">
                    <span class="skill-name">${sk.icon || ''} ${sk.name}</span>
                    <span class="skill-desc">${sk.desc}${sk.hits > 1 ? ' (' + sk.hits + ' hits)' : ''}</span>
                </div>
                <span class="skill-cost">
                    ${disabled
                        ? (cd > 0 ? 'CD:' + cd : 'No SP')
                        : sk.spCost + ' SP'}
                </span>`;

            if (!disabled) {
                d.addEventListener('click', () => { this.hideSubMenus(); combat.playerSkill(id); });
            }
            container.appendChild(d);
        });

        if (!p.skills.length) {
            container.innerHTML = '<div style="color:var(--text-secondary);font-size:12px">No skills available.</div>';
        }
    },

    _renderCombatItems() {
        const p   = this.player;
        const container = el('combat-item-list');
        container.innerHTML = '';

        const usable = p.inventory.filter(i => i.effect);
        if (!usable.length) {
            container.innerHTML = '<div style="color:var(--text-secondary);font-size:12px">No items.</div>';
            return;
        }

        usable.forEach(item => {
            const origIdx = p.inventory.indexOf(item);
            const d = document.createElement('div');
            d.className = 'item-item';
            d.innerHTML = `
                <div>
                    <div class="item-name">${item.icon || ''} ${item.name}</div>
                    <div class="item-desc-small">${item.desc}</div>
                </div>
                <span class="item-qty">×${item.qty || 1}</span>`;
            d.addEventListener('click', () => { this.hideSubMenus(); combat.playerItem(origIdx); });
            container.appendChild(d);
        });
    },

    /* ── NOTIFICATION TOAST ── */
    _notify(msg) {
        const existing = document.querySelector('.notification');
        if (existing) existing.remove();

        const d = document.createElement('div');
        d.className = 'notification';
        d.textContent = msg;
        document.getElementById('app').appendChild(d);
        setTimeout(() => d.remove(), 2200);
    },

    /* ── EVENT BINDING ── */
    _bindEvents() {
        /* Title */
        el('btn-start').addEventListener('click', () => this.showScreen('create'));

        /* Create */
        el('player-name').addEventListener('input', () => this._validateCreate());
        document.querySelectorAll('.class-card').forEach(c =>
            c.addEventListener('click', () => {
                document.querySelectorAll('.class-card').forEach(x => x.classList.remove('selected'));
                c.classList.add('selected');
                this._validateCreate();
            })
        );
        el('btn-create').addEventListener('click', () => this.createCharacter());

        /* Town actions */
        el('btn-shop').addEventListener('click',   () => this.showPanel('shop'));
        el('btn-rest').addEventListener('click',   () => this.rest());
        el('btn-dungeon').addEventListener('click',() => this.showPanel('dungeon'));
        el('btn-status').addEventListener('click', () => this.showPanel('status'));

        /* Dungeon actions */
        el('btn-explore').addEventListener('click',   () => this.explore());
        el('btn-boss').addEventListener('click',      () => this.challengeBoss());
        el('btn-back-town').addEventListener('click', () => this.showPanel('town'));

        /* Combat actions */
        el('btn-attack').addEventListener('click', () => combat.playerAttack());
        el('btn-skills').addEventListener('click', () => this.showSkillMenu());
        el('btn-item').addEventListener('click',   () => this.showItemMenu());
        el('btn-flee').addEventListener('click',   () => combat.playerFlee());

        el('btn-back-skills').addEventListener('click', () => this.hideSubMenus());
        el('btn-back-items').addEventListener('click',  () => this.hideSubMenus());

        /* Shop / Status back */
        el('btn-back-shop').addEventListener('click',   () => this.showPanel('town'));
        el('btn-back-status').addEventListener('click', () => this.showPanel('town'));

        /* Shop tabs */
        document.querySelectorAll('.shop-tab').forEach(t =>
            t.addEventListener('click', () => this._renderShop(t.dataset.tab))
        );

        /* Reward / Level-up / Floor clear continue */
        el('btn-reward-continue').addEventListener('click', () => this.afterReward());
        el('btn-levelup-continue').addEventListener('click',() => this.afterLevelUp());
        el('btn-floor-continue').addEventListener('click',  () => this.advanceFloor());

        /* Game over / Victory restart */
        el('btn-restart').addEventListener('click',   () => this.showScreen('title'));
        el('btn-play-again').addEventListener('click',() => this.showScreen('title'));
    }
};

/* ──────────────────────────────────────────
   COMBAT ENGINE
   ────────────────────────────────────────── */
const combat = {
    active: false,
    enemy: null,
    playerFirst: true,
    turnBlocked: false,  // prevents double-click spam

    start(enemy) {
        this.enemy       = JSON.parse(JSON.stringify(enemy)); // deep copy
        this.active      = true;
        this.turnBlocked = false;

        /* Decide turn order */
        this.playerFirst = (game.player.totalAgi || game.player.agi) >=
                           (this.enemy.agi || rand(4, 10));

        /* Render enemy */
        el('enemy-name').textContent = this.enemy.name;
        el('enemy-sprite').textContent = this.enemy.sprite;
        el('enemy-sprite').className = 'enemy-sprite' + (this.enemy.isBoss ? ' boss-sprite' : '');

        const badge = el('enemy-type-badge');
        if (this.enemy.isFinalBoss) {
            badge.textContent = '⚠ FINAL BOSS';
            badge.className   = 'enemy-type-badge boss-badge';
        } else if (this.enemy.isBoss) {
            badge.textContent = '☠ FLOOR BOSS';
            badge.className   = 'enemy-type-badge boss-badge';
        } else {
            badge.textContent = 'MONSTER';
            badge.className   = 'enemy-type-badge';
        }

        this._clearLog();
        if (this.enemy.desc) {
            this._log('system', '"' + this.enemy.desc + '"');
        }
        if (!this.playerFirst) {
            this._log('system', 'Enemy moves first!');
        }

        this._refreshCombatBars();
        this._renderEnemyStatus();
        this._renderPlayerStatus();
        game.hideSubMenus();
        this._setActionsEnabled(true);

        /* If enemy goes first */
        if (!this.playerFirst) {
            setTimeout(() => this._enemyTurn(), 600);
        }
    },

    /* ── PLAYER ACTIONS ── */
    playerAttack() {
        if (!this.active || this.turnBlocked) return;
        this.turnBlocked = true;
        this._setActionsEnabled(false);

        const p = game.player;
        const dmg = this._calcDamage(p.totalAtk, this.enemy.def);
        const crit = Math.random() < 0.1;
        const finalDmg = crit ? Math.floor(dmg * 1.75) : dmg;

        this.enemy.hp = Math.max(0, this.enemy.hp - finalDmg);
        this._refreshCombatBars();

        if (crit) this._log('critical', '💥 Critical hit! ' + p.name + ' deals ' + finalDmg + ' damage!');
        else       this._log('player',   p.name + ' attacks for ' + finalDmg + ' damage.');

        if (this._checkEnemyDead()) return;

        setTimeout(() => this._enemyTurn(), 700);
    },

    playerSkill(skillId) {
        if (!this.active || this.turnBlocked) return;
        const p  = game.player;
        const sk = DATA.skills[skillId];
        if (!sk || p.sp < sk.spCost) return;
        if ((p.cooldowns[skillId] || 0) > 0) return;

        this.turnBlocked = true;
        this._setActionsEnabled(false);
        p.sp = Math.max(0, p.sp - sk.spCost);
        p.cooldowns[skillId] = sk.cooldown;
        this._refreshCombatBars();

        this._resolveSkill(sk, skillId);
    },

    _resolveSkill(sk, id) {
        const p = game.player;

        if (sk.type === 'buff') {
            p.buffs.push({ stat: sk.stat, amount: sk.buffAmount, duration: sk.buffDuration });
            game._calcStats();
            this._renderPlayerStatus();
            this._log('skill', '✦ ' + sk.name + '! DEF greatly increased for ' + sk.buffDuration + ' turns.');
            setTimeout(() => this._enemyTurn(), 700);
            return;
        }

        if (sk.type === 'buff_debuff') {
            p.buffs.push({ stat: 'atk', amount: sk.atkBuff,   duration: sk.duration });
            p.buffs.push({ stat: 'def', amount: sk.defDebuff,  duration: sk.duration });
            p.statusEffects.push({ name: 'Berserk', type: 'berserk', duration: sk.duration });
            game._calcStats();
            this._renderPlayerStatus();
            this._log('skill', '🔥 ' + sk.name + '! ATK +' + sk.atkBuff + ', DEF ' + sk.defDebuff + ' for ' + sk.duration + ' turns!');
            setTimeout(() => this._enemyTurn(), 700);
            return;
        }

        /* Damage-based skills */
        let totalDmg = 0;
        const pierce = sk.pierce || 0;
        const effDef  = Math.floor(this.enemy.def * (1 - pierce));

        for (let i = 0; i < sk.hits; i++) {
            const dmg = this._calcDamage(p.totalAtk, effDef);
            const finalDmg = Math.floor(dmg * sk.multiplier);
            this.enemy.hp = Math.max(0, this.enemy.hp - finalDmg);
            totalDmg += finalDmg;
        }

        this._refreshCombatBars();
        const hitStr = sk.hits > 1 ? ' (' + sk.hits + ' hits)' : '';
        this._log('skill', '✦ ' + sk.name + hitStr + '! ' + totalDmg + ' total damage!');

        /* Stun chance */
        if (sk.type === 'damage_stun' && Math.random() < (sk.stunChance || 0)) {
            this.enemy.statusEffects = this.enemy.statusEffects || [];
            this.enemy.statusEffects.push({ name: 'Stun', type: 'stun', duration: 1 });
            this._log('system', this.enemy.name + ' is stunned!');
            this._renderEnemyStatus();
        }

        if (this._checkEnemyDead()) return;
        setTimeout(() => this._enemyTurn(), 700);
    },

    playerItem(invIdx) {
        if (!this.active || this.turnBlocked) return;
        const p    = game.player;
        const item = p.inventory[invIdx];
        if (!item) return;

        this.turnBlocked = true;
        this._setActionsEnabled(false);

        let msg = '';
        if (item.effect === 'heal') {
            const amt = Math.min(item.value, p.maxHp - p.hp);
            p.hp += amt;
            msg = '💊 ' + p.name + ' uses ' + item.name + '. Restored ' + amt + ' HP.';
            this._log('heal', msg);
        } else if (item.effect === 'sp') {
            const amt = Math.min(item.value, p.maxSp - p.sp);
            p.sp += amt;
            this._log('heal', '🔷 ' + p.name + ' uses ' + item.name + '. Restored ' + amt + ' SP.');
        } else if (item.effect === 'full_heal') {
            p.hp = p.maxHp;
            p.sp = p.maxSp;
            this._log('heal', '🌟 ' + p.name + ' uses ' + item.name + '. Fully restored!');
        } else if (item.effect === 'cure') {
            p.statusEffects = p.statusEffects.filter(e => e.type !== item.value);
            this._log('heal', '💚 ' + p.name + ' uses ' + item.name + '.');
        }

        game._consumeItem(invIdx);
        this._refreshCombatBars();
        this._renderPlayerStatus();

        setTimeout(() => this._enemyTurn(), 700);
    },

    playerFlee() {
        if (!this.active || this.turnBlocked) return;
        if (this.enemy.isBoss) { this._log('system', 'You cannot flee from a boss!'); return; }

        const p       = game.player;
        const fleeChance = 0.45 + (p.totalAgi - (this.enemy.agi || 8)) * 0.03;
        const success    = Math.random() < clamp(fleeChance, 0.15, 0.85);

        if (success) {
            this._log('system', 'You fled successfully!');
            this._endCombat(false, true);
        } else {
            this._log('system', 'Failed to flee!');
            this.turnBlocked = true;
            this._setActionsEnabled(false);
            setTimeout(() => this._enemyTurn(), 700);
        }
    },

    /* ── ENEMY TURN ── */
    _enemyTurn() {
        const e = this.enemy;
        const p = game.player;

        /* Check stun */
        if (e.statusEffects && e.statusEffects.some(ef => ef.type === 'stun')) {
            e.statusEffects = e.statusEffects.filter(ef => {
                ef.duration--;
                return ef.duration > 0;
            });
            this._log('system', e.name + ' is stunned and cannot act!');
            this._renderEnemyStatus();
            this._afterEnemyTurn();
            return;
        }

        /* Tick enemy status effects */
        if (e.statusEffects) {
            e.statusEffects.forEach(ef => {
                if (ef.type === 'poison') {
                    const dot = Math.max(1, Math.floor(e.maxHp * 0.03));
                    e.hp = Math.max(0, e.hp - dot);
                    this._log('system', e.name + ' takes ' + dot + ' poison damage.');
                }
            });
            e.statusEffects = e.statusEffects.filter(ef => { ef.duration--; return ef.duration > 0; });
            this._renderEnemyStatus();
        }

        if (this._checkEnemyDead()) return;

        /* Boss occasionally uses special skill */
        if (e.isBoss && Math.random() < 0.28 && e.bossSkills && e.bossSkills.length) {
            const skillName = e.bossSkills[rand(0, e.bossSkills.length - 1)];
            const dmg       = this._calcDamage(e.atk, p.totalDef);
            const bossHit   = Math.floor(dmg * 1.6);
            p.hp = Math.max(0, p.hp - bossHit);
            this._log('enemy', '💥 ' + e.name + ' uses ' + skillName + ' for ' + bossHit + ' damage!');
        } else {
            /* Normal attack */
            const miss = Math.random() < Math.max(0, (p.totalAgi - (e.agi || 8)) * 0.02);
            if (miss) {
                this._log('miss', e.name + ' attacks... and misses!');
            } else {
                const dmg     = this._calcDamage(e.atk, p.totalDef);
                const crit    = Math.random() < 0.07;
                const finalDmg = crit ? Math.floor(dmg * 1.7) : dmg;
                p.hp = Math.max(0, p.hp - finalDmg);
                if (crit) this._log('critical', e.name + ' lands a critical hit for ' + finalDmg + ' damage!');
                else       this._log('enemy',    e.name + ' attacks for ' + finalDmg + ' damage.');
            }
        }

        /* Tick player poison */
        if (p.statusEffects && p.statusEffects.some(ef => ef.type === 'poison')) {
            const dot = Math.max(1, Math.floor(p.maxHp * 0.03));
            p.hp = Math.max(0, p.hp - dot);
            this._log('enemy', p.name + ' takes ' + dot + ' poison damage!');
        }

        this._refreshCombatBars();
        this._renderPlayerStatus();
        game._updateHUD();

        if (p.hp <= 0) {
            this._endCombat(false, false);
            return;
        }

        this._afterEnemyTurn();
    },

    _afterEnemyTurn() {
        this.turnBlocked = false;
        this._setActionsEnabled(true);
    },

    /* ── DAMAGE FORMULA ── */
    _calcDamage(atk, def) {
        const base = Math.max(1, atk - Math.floor(def * 0.6));
        return Math.floor(base * (0.9 + Math.random() * 0.2));
    },

    /* ── CHECK DEAD ── */
    _checkEnemyDead() {
        if (this.enemy.hp <= 0) {
            this._refreshCombatBars();
            this._log('system', this.enemy.name + ' has been defeated!');
            setTimeout(() => this._endCombat(true, false), 600);
            return true;
        }
        return false;
    },

    _endCombat(won, fled) {
        this.active = false;
        this._setActionsEnabled(false);

        if (won) {
            game.onCombatEnd(true, this.enemy);
        } else if (fled) {
            game.showScreen('main');
            game.showPanel('dungeon');
            game._refreshDungeon();
        } else {
            game.onCombatEnd(false, this.enemy);
        }
    },

    /* ── UI HELPERS ── */
    _refreshCombatBars() {
        const p = game.player;
        const e = this.enemy;

        /* Enemy bars */
        const ePct = clamp((e.hp / e.maxHp) * 100, 0, 100);
        el('enemy-hp-bar').style.width  = ePct + '%';
        el('enemy-hp-text').textContent = e.hp + '/' + e.maxHp;

        /* Player bars */
        const hpPct = clamp((p.hp / p.maxHp) * 100, 0, 100);
        const spPct = clamp((p.sp / p.maxSp) * 100, 0, 100);
        el('combat-hp-bar').style.width  = hpPct + '%';
        el('combat-hp-text').textContent = p.hp + '/' + p.maxHp;
        el('combat-sp-bar').style.width  = spPct + '%';
        el('combat-sp-text').textContent = p.sp + '/' + p.maxSp;
    },

    _renderEnemyStatus() {
        const row = el('enemy-status-row');
        if (!this.enemy || !this.enemy.statusEffects) { row.innerHTML = ''; return; }
        row.innerHTML = this.enemy.statusEffects.map(ef =>
            `<span class="status-badge effect-${ef.type}">${ef.name}</span>`
        ).join('');
    },

    _renderPlayerStatus() {
        const p   = game.player;
        const row = el('player-status-row');
        row.innerHTML = p.statusEffects.map(ef =>
            `<span class="status-badge effect-${ef.type}">${ef.name}</span>`
        ).concat(p.buffs.map(b =>
            `<span class="status-badge effect-ironwall">${b.stat.toUpperCase()} ${b.amount > 0 ? '+' : ''}${b.amount} (${b.duration})</span>`
        )).join('');
    },

    _clearLog() {
        el('battle-log').innerHTML = '';
    },

    _log(cls, msg) {
        const log = el('battle-log');
        const d   = document.createElement('div');
        d.className = 'log-' + cls;
        d.textContent = msg;
        log.appendChild(d);
        log.scrollTop = log.scrollHeight;
    },

    _setActionsEnabled(enabled) {
        const actions = el('combat-actions');
        actions.style.display = enabled ? '' : 'none';
    }
};

/* ──────────────────────────────────────────
   BOOT
   ────────────────────────────────────────── */
document.addEventListener('DOMContentLoaded', () => game.init());

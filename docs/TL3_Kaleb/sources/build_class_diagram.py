#!/usr/bin/env python3
"""UML class diagram for Feature 4 — Upgrade & Loot (Champion internals only)."""

from __future__ import annotations

import os
from pathlib import Path

os.environ.setdefault("MPLCONFIGDIR", "/tmp/mplconfig")

import matplotlib.pyplot as plt
from matplotlib.patches import FancyArrowPatch, FancyBboxPatch, Polygon, Rectangle

ROOT = Path(__file__).resolve().parent.parent
IMG = ROOT / "general_diagrams"
IMG.mkdir(exist_ok=True)

NAVY = "#2F3E4E"
TITLE_C = "#1E2761"
TEXT = "#2C3E50"
MUTED = "#5A6A7A"
WHITE = "#FFFFFF"
PEACH = "#F3D2B5"
PEACH_EDGE = "#C4A882"
GRAY = "#E8E8E8"
GRAY_EDGE = "#8A94A0"


def save(fig, name: str) -> Path:
    path = IMG / name
    fig.savefig(path, dpi=220, bbox_inches="tight", facecolor="white", pad_inches=0.22)
    plt.close(fig)
    return path


def text(ax, x, y, s, size=8.2, weight="normal", color=TEXT, ha="center", va="center", style="normal"):
    ax.text(
        x,
        y,
        s,
        fontsize=size,
        fontweight=weight,
        color=color,
        ha=ha,
        va=va,
        fontstyle=style,
        zorder=6,
        clip_on=False,
    )


class Box:
    def __init__(self, cx, cy, w, h):
        self.cx, self.cy, self.w, self.h = cx, cy, w, h

    @property
    def x(self):
        return self.cx - self.w / 2

    @property
    def y(self):
        return self.cy - self.h / 2

    def top(self, inset=0):
        return (self.cx, self.y + self.h - inset)

    def bottom(self, inset=0):
        return (self.cx, self.y + inset)

    def left(self, inset=0):
        return (self.x + inset, self.cy)

    def right(self, inset=0):
        return (self.x + self.w - inset, self.cy)

    def edge_toward(self, other, inset=0.0):
        dx, dy = other.cx - self.cx, other.cy - self.cy
        if abs(dx) * self.h >= abs(dy) * self.w:
            return (self.right()[0] - inset, self.cy) if dx > 0 else (self.left()[0] + inset, self.cy)
        return (self.cx, self.top()[1] - inset) if dy > 0 else (self.cx, self.bottom()[1] + inset)


def class_box(
    ax,
    cx,
    cy,
    w,
    name,
    attrs,
    ops,
    *,
    stereotype=None,
    abstract=False,
    fill=WHITE,
    edge=NAVY,
    header_fill=None,
):
    name_size = 8.6
    body_size = 7.15
    stereo_size = 7.0
    header_h = 4.55 if stereotype else 3.2
    line_h = 1.55
    show_attrs = bool(attrs)
    attr_h = (len(attrs) * line_h + 1.1) if show_attrs else 0.0
    ops_h = max(len(ops), 1) * line_h + 1.1
    h = header_h + attr_h + ops_h
    box = Box(cx, cy, w, h)
    x, y = box.x, box.y

    ax.add_patch(
        FancyBboxPatch(
            (x, y),
            w,
            h,
            boxstyle="round,pad=0.08,rounding_size=0.18",
            facecolor=fill,
            edgecolor=edge,
            linewidth=1.45,
            zorder=3,
        )
    )
    if header_fill:
        ax.add_patch(Rectangle((x, y + h - header_h), w, header_h, facecolor=header_fill, edgecolor="none", zorder=3.2))
        ax.plot([x, x + w], [y + h - header_h, y + h - header_h], color=edge, lw=1.15, zorder=4)

    if stereotype:
        text(ax, cx, y + h - 1.2, stereotype, size=stereo_size, style="italic", color=MUTED)
        text(ax, cx, y + h - 2.9, name, size=name_size, weight="bold", style="italic" if abstract else "normal")
    else:
        text(ax, cx, y + h - 1.55, name, size=name_size, weight="bold", style="italic" if abstract else "normal")

    split_header = y + ops_h + attr_h
    ax.plot([x + 0.15, x + w - 0.15], [split_header, split_header], color=edge, lw=1.05, zorder=4)
    if show_attrs:
        split_attr = y + ops_h
        ax.plot([x + 0.15, x + w - 0.15], [split_attr, split_attr], color=edge, lw=1.05, zorder=4)
        top = split_header - 0.85
        for i, line in enumerate(attrs):
            text(ax, x + 0.7, top - i * line_h, line, size=body_size, ha="left")
        ops_top = split_attr - 0.85
    else:
        ops_top = split_header - 0.85

    for i, line in enumerate(ops):
        italic = "{virtual}" in line or (abstract and "ApplyEffect" in line)
        text(ax, x + 0.7, ops_top - i * line_h, line, size=body_size, ha="left", style="italic" if italic else "normal")

    return box


def enum_box(ax, cx, cy, w, name, values, stereotype="«enumeration»", fill=WHITE, edge=NAVY):
    line_h = 1.55
    header_h = 4.6
    body_h = len(values) * line_h + 1.2
    h = header_h + body_h
    box = Box(cx, cy, w, h)
    x, y = box.x, box.y
    ax.add_patch(
        FancyBboxPatch(
            (x, y),
            w,
            h,
            boxstyle="round,pad=0.08,rounding_size=0.18",
            facecolor=fill,
            edgecolor=edge,
            linewidth=1.45,
            zorder=3,
        )
    )
    text(ax, cx, y + h - 1.2, stereotype, size=7.0, style="italic", color=MUTED)
    text(ax, cx, y + h - 2.85, name, size=8.6, weight="bold")
    ax.plot([x + 0.15, x + w - 0.15], [y + body_h, y + body_h], color=edge, lw=1.05, zorder=4)
    for i, val in enumerate(values):
        text(ax, x + 0.7, y + body_h - 0.95 - i * line_h, val, size=7.15, ha="left")
    return box


def line(ax, p1, p2, color=NAVY, lw=1.2, ls="solid"):
    ax.plot([p1[0], p2[0]], [p1[1], p2[1]], color=color, lw=lw, linestyle=ls, zorder=2, solid_capstyle="butt")


def polyline(ax, pts, color=NAVY, lw=1.2, ls="solid"):
    xs, ys = [p[0] for p in pts], [p[1] for p in pts]
    ax.plot(xs, ys, color=color, lw=lw, linestyle=ls, zorder=2, solid_capstyle="butt")


def _unit(p1, p2):
    dx, dy = p2[0] - p1[0], p2[1] - p1[1]
    length = max((dx * dx + dy * dy) ** 0.5, 1e-6)
    return dx / length, dy / length, length


def hollow_triangle(ax, tip, from_pt, size=2.15):
    ux, uy, _ = _unit(from_pt, tip)
    px, py = -uy, ux
    left = (tip[0] - ux * size + px * size * 0.58, tip[1] - uy * size + py * size * 0.58)
    right = (tip[0] - ux * size - px * size * 0.58, tip[1] - uy * size - py * size * 0.58)
    ax.add_patch(
        Polygon(
            [tip, left, right],
            closed=True,
            facecolor=WHITE,
            edgecolor=NAVY,
            linewidth=1.25,
            zorder=5,
        )
    )
    return (tip[0] - ux * size, tip[1] - uy * size)


def open_arrow(ax, tip, from_pt, size=1.85):
    ux, uy, _ = _unit(from_pt, tip)
    px, py = -uy, ux
    left = (tip[0] - ux * size + px * size * 0.5, tip[1] - uy * size + py * size * 0.5)
    right = (tip[0] - ux * size - px * size * 0.5, tip[1] - uy * size - py * size * 0.5)
    ax.plot([left[0], tip[0], right[0]], [left[1], tip[1], right[1]], color=NAVY, lw=1.2, zorder=5)


def diamond(ax, at, along, filled=True, size=1.55):
    ux, uy, _ = _unit(at, along)
    px, py = -uy, ux
    pts = [
        at,
        (at[0] + ux * size + px * size * 0.55, at[1] + uy * size + py * size * 0.55),
        (at[0] + ux * size * 2.0, at[1] + uy * size * 2.0),
        (at[0] + ux * size - px * size * 0.55, at[1] + uy * size - py * size * 0.55),
    ]
    ax.add_patch(
        Polygon(
            pts,
            closed=True,
            facecolor=NAVY if filled else WHITE,
            edgecolor=NAVY,
            linewidth=1.2,
            zorder=5,
        )
    )
    return pts[2]


def generalize(ax, child: Box, parent: Box, bus_y=None):
    """Solid line, hollow triangle at parent (Pearson generalization)."""
    if bus_y is None:
        start = child.top()
        end = parent.bottom()
        neck = hollow_triangle(ax, end, start)
        line(ax, start, neck)
        return
    start = child.top()
    bus = (child.cx, bus_y)
    parent_pt = parent.bottom()
    neck = hollow_triangle(ax, parent_pt, (parent.cx, bus_y))
    polyline(ax, [start, bus, (parent.cx, bus_y), neck])


def associate(ax, a: Box, b: Box, label=None, lxy=None, mult_a=None, mxy_a=None, mult_b=None, mxy_b=None, via=None, start=None, end=None):
    p1 = start or a.edge_toward(b)
    p2 = end or b.edge_toward(a)
    pts = [p1] + (via or []) + [p2]
    polyline(ax, pts)
    if label and lxy:
        text(ax, lxy[0], lxy[1], label, size=6.6, color=MUTED, style="italic")
    if mult_a:
        mx, my = mxy_a if mxy_a else (p1[0] + 0.7, p1[1] + 1.1)
        text(ax, mx, my, mult_a, size=6.4, color=MUTED)
    if mult_b:
        mx, my = mxy_b if mxy_b else (p2[0] - 0.7, p2[1] + 1.1)
        text(ax, mx, my, mult_b, size=6.4, color=MUTED)


def compose(ax, whole: Box, part: Box, label=None, lxy=None, via=None, start=None, end=None):
    p1 = start or whole.edge_toward(part)
    p2 = end or part.edge_toward(whole)
    first = via[0] if via else p2
    d_end = diamond(ax, p1, first, filled=True)
    pts = [d_end] + (via or []) + [p2]
    polyline(ax, pts)
    if label and lxy:
        text(ax, lxy[0], lxy[1], label, size=6.6, color=MUTED, style="italic")


def depend(ax, src: Box, dst: Box, label=None, lxy=None, via=None):
    p1 = src.edge_toward(dst)
    p2 = dst.edge_toward(src)
    pts = [p1] + (via or []) + [p2]
    polyline(ax, pts, ls=(0, (4.5, 2.4)))
    prev = pts[-2]
    open_arrow(ax, pts[-1], prev)
    if label and lxy:
        text(ax, lxy[0], lxy[1], label, size=6.6, color=MUTED, style="italic")


def note_box(ax, cx, cy, w, h, title, lines):
    x, y = cx - w / 2, cy - h / 2
    ax.add_patch(Rectangle((x, y), w, h, facecolor="#F7F4EE", edgecolor=NAVY, linewidth=1.15, zorder=3))
    fold = 1.35
    ax.add_patch(
        Polygon(
            [(x + w - fold, y + h), (x + w, y + h - fold), (x + w - fold, y + h - fold)],
            closed=True,
            facecolor="#E8E0D4",
            edgecolor=NAVY,
            linewidth=0.9,
            zorder=4,
        )
    )
    ax.plot([x + w - fold, x + w], [y + h, y + h - fold], color=NAVY, lw=0.9, zorder=5)
    text(ax, x + 0.7, y + h - 1.25, title, size=7.1, weight="bold", ha="left")
    for i, line in enumerate(lines):
        text(ax, x + 0.7, y + h - 2.7 - i * 1.35, line, size=6.6, ha="left", color=TEXT)


def legend_strip(ax, x, y):
    items = [
        "▷ generalization",
        "—— association",
        "◆ composition",
        "- - ▶ dependency",
        "gray = other feature",
    ]
    text(ax, x, y, "Notation:", size=6.8, weight="bold", ha="left", color=TITLE_C)
    cursor = x + 12.2
    for mark in items:
        text(ax, cursor, y, mark, size=6.6, ha="left", color=MUTED)
        cursor += 24.6


def draw_class_diagram():
    fig, ax = plt.subplots(figsize=(17.4, 11.0))
    ax.set_xlim(0, 176)
    ax.set_ylim(-1, 106)
    ax.set_aspect("equal")
    ax.axis("off")
    fig.patch.set_facecolor("white")
    ax.set_facecolor("white")

    text(
        ax,
        88,
        104.2,
        "Class Diagram — Feature 4 Upgrade & Loot System",
        size=13.4,
        weight="bold",
        color=TITLE_C,
    )
    text(
        ax,
        88,
        101.35,
        "Champion internals only. Gray types belong to Feature 2 or Feature 7; they are dependencies, not classes this feature owns.",
        size=7.5,
        color=MUTED,
    )
    legend_strip(ax, 8.5, 98.55)

    loot_system = class_box(
        ax,
        23,
        81.4,
        40,
        "LootSystem",
        [],
        [
            "+ ApplyItem(item : Item, stats : PlayerStats) : void",
            "+ RollDrop(roomType : RoomType) : Item",
            "+ SaveState() : ItemSnapshot",
            "+ RestoreState(snap : ItemSnapshot) : void",
        ],
        stereotype="«Feature 4 public API»",
        fill=PEACH,
        edge=PEACH_EDGE,
        header_fill="#F8E6D0",
    )

    item = class_box(
        ax,
        92,
        84.0,
        42,
        "Item",
        [
            "+ itemId : string",
            "+ kind : ItemKind",
        ],
        [
            "+ ApplyEffect(stats : PlayerStats) : void  {virtual}",
            "+ Clone() : Item  «Prototype»",
        ],
        stereotype="«abstract»",
        abstract=True,
        fill=PEACH,
        edge=PEACH_EDGE,
        header_fill="#F8E6D0",
    )

    item_kind = enum_box(
        ax,
        152,
        83.6,
        26,
        "ItemKind",
        ["None", "WeaponUpgrade", "ArmorUpgrade", "Consumable", "Relic"],
    )

    loot_table = class_box(
        ax,
        23,
        56.4,
        36,
        "LootTable",
        [
            "− dropRates : Map<RoomType, float>",
            "     {Combat 0.35, Corridor 0.10, T/B 1.00}",
            "− weights : Map<RoomType, (ItemId, int)[*]>",
        ],
        [
            "+ RollDrop(roomType : RoomType) : Item",
        ],
    )

    weapon = class_box(
        ax,
        63,
        48.6,
        27,
        "WeaponUpgradeItem",
        ["+ attackDelta : int   {5 | 8}"],
        ["+ ApplyEffect(stats : PlayerStats) : void"],
    )
    armor = class_box(
        ax,
        96,
        47.8,
        28,
        "ArmorUpgradeItem",
        [
            "+ defenseDelta : int     {3 | 5}",
            "+ maxHealthDelta : int   {10 | 20}",
        ],
        ["+ ApplyEffect(stats : PlayerStats) : void"],
    )
    consumable = class_box(
        ax,
        130,
        47.0,
        30,
        "ConsumableItem",
        [
            "+ consumableId : string",
            "+ durationRemaining : float  {10.0}",
            "+ attackDelta : int = 8",
        ],
        [
            "+ ApplyEffect(stats : PlayerStats) : void",
            "+ Revert(stats : PlayerStats) : void",
        ],
    )
    relic = class_box(
        ax,
        162,
        47.8,
        27,
        "RelicItem",
        [
            "+ relicId : string",
            "+ moveSpeedDelta : float = 0.15",
            "+ critChanceDelta : int = 10",
        ],
        ["+ ApplyEffect(stats : PlayerStats) : void"],
    )

    room_type = enum_box(
        ax,
        23,
        22.4,
        32,
        "RoomType",
        ["CombatRoom", "TreasureRoom", "CorridorRoom", "BossRoom"],
        stereotype="«enumeration / Feature 2»",
        fill=GRAY,
        edge=GRAY_EDGE,
    )

    memento = class_box(
        ax,
        68,
        22.0,
        33,
        "ItemMemento",
        [],
        [
            "+ SaveState() : ItemSnapshot",
            "+ RestoreState(snap : ItemSnapshot) : void",
        ],
        stereotype="«GoF Memento»",
    )

    snapshot = class_box(
        ax,
        110,
        21.2,
        36,
        "ItemSnapshot",
        [
            "+ equippedItemIds : string[*]",
            "+ consumableTimers : (string, float)[*]",
            "+ statBaseline : PlayerStats",
            "+ schemaVersion : int = 1",
        ],
        [],
    )

    stats = class_box(
        ax,
        154,
        21.4,
        32,
        "PlayerStats",
        [
            "+ maxHealth : int = 100",
            "+ currentHealth : int = 100",
            "+ attack : int = 10",
            "+ defense : int = 0",
            "+ moveSpeed : float = 1.00",
            "+ critChancePercent : int = 0",
        ],
        [],
        stereotype="«external / Feature 7»",
        fill=GRAY,
        edge=GRAY_EDGE,
    )

    # Public API to internals — keep lines off the inheritance bus (y ≈ 61).
    compose(
        ax,
        loot_system,
        loot_table,
        start=loot_system.bottom(),
        end=loot_table.top(),
        label="RollDrop",
        lxy=(12.8, 68.4),
    )
    associate(
        ax,
        loot_system,
        item,
        start=(loot_system.x + loot_system.w, loot_system.cy + 2.4),
        end=(item.x, item.cy + 2.4),
        label="loadout",
        lxy=(54.8, 90.0),
        mult_b="0..*",
        mxy_b=(73.2, 88.2),
    )
    gutter_x = loot_system.x + loot_system.w + 2.8
    compose(
        ax,
        loot_system,
        memento,
        start=(loot_system.x + loot_system.w, loot_system.y + 1.8),
        end=memento.left(),
        label="save / restore",
        lxy=(gutter_x + 6.4, 33.2),
        via=[
            (gutter_x, loot_system.y + 1.8),
            (gutter_x, 32.8),
            (memento.x - 0.15, 32.8),
        ],
    )
    associate(
        ax,
        loot_table,
        item,
        start=loot_table.right(),
        end=(item.x, item.y + 1.4),
        label="clones prototype",
        lxy=(56.2, 66.6),
        mult_b="1..*",
        mxy_b=(73.4, 70.8),
        via=[(52.6, loot_table.cy), (52.6, 71.4), (item.x, 71.4)],
    )
    depend(ax, loot_table, room_type, label="«use»", lxy=(12.4, 39.2))
    associate(ax, item, item_kind, label="kind", lxy=(123.8, 89.4))

    bus_y = 62.4
    for child in (weapon, armor, consumable, relic):
        generalize(ax, child, item, bus_y=bus_y)

    associate(ax, memento, snapshot, label="stores", lxy=(88.6, 25.6), mult_b="1", mxy_b=(92.2, 18.6))
    associate(ax, snapshot, stats, label="statBaseline", lxy=(132.4, 25.4))

    note_box(
        ax,
        55.5,
        4.15,
        74,
        6.2,
        "Null Object — not a fifth Item subclass",
        [
            "NullItem is ItemId = \"NULL\", Kind = None. ApplyItem is a no-op.",
            "CombatRoom / CorridorRoom may return it; TreasureRoom / BossRoom never do.",
        ],
    )
    note_box(
        ax,
        132.5,
        4.15,
        70,
        6.2,
        "Out of this diagram",
        [
            "Champion internal classes: Item + four subtypes, LootTable, ItemMemento.",
            "HUD, map, combat, points, and boss phases stay on Features 1–3 and 5–7.",
        ],
    )

    return save(fig, "class_diagram.png")


if __name__ == "__main__":
    path = draw_class_diagram()
    print("Wrote", path, path.stat().st_size)

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
        italic = "{virtual}" in line or "{override}" in line or (abstract and "ApplyEffect" in line)
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
        "− field   + get/set",
    ]
    text(ax, x, y, "Notation:", size=6.8, weight="bold", ha="left", color=TITLE_C)
    cursor = x + 12.2
    for mark in items:
        text(ax, cursor, y, mark, size=6.6, ha="left", color=MUTED)
        cursor += 28.0


def draw_class_diagram():
    fig, ax = plt.subplots(figsize=(18.4, 12.6))
    ax.set_xlim(0, 186)
    ax.set_ylim(-2, 120)
    ax.set_aspect("equal")
    ax.axis("off")
    fig.patch.set_facecolor("white")
    ax.set_facecolor("white")

    text(
        ax,
        93,
        118.0,
        "Class Diagram — Feature 4 Upgrade & Loot System",
        size=13.4,
        weight="bold",
        color=TITLE_C,
    )
    text(
        ax,
        93,
        115.1,
        "Feature 4 classes only. LootSystem is the Unity hook (MonoBehaviour). All other Feature 4 types are plain C#.",
        size=7.5,
        color=MUTED,
    )
    legend_strip(ax, 48.0, 112.2)

    loot_system = class_box(
        ax,
        23,
        93.4,
        40,
        "LootSystem",
        [
            "− table : LootTable",
            "− loadout : Loadout",
            "− heldMemento : ItemMemento",
            "+ Table { get; }",
            "+ Loadout { get; }",
        ],
        [
            "+ ApplyItem(item : Item, stats : PlayerStats) : void",
            "+ RollDrop(roomType : RoomType) : Item",
            "+ SaveState() : ItemMemento",
            "+ RestoreState(m : ItemMemento) : void",
            "+ Update() : void",
        ],
        stereotype="«Caretaker»  MonoBehaviour",
        fill=PEACH,
        edge=PEACH_EDGE,
        header_fill="#F8E6D0",
    )

    mono = class_box(
        ax,
        23,
        111.2,
        28,
        "MonoBehaviour",
        [],
        [],
        stereotype="«Unity»",
        fill=GRAY,
        edge=GRAY_EDGE,
    )
    generalize(ax, loot_system, mono)

    item = class_box(
        ax,
        98,
        95.4,
        46,
        "Item",
        [
            "− itemId : string",
            "− kind : ItemKind",
            "+ ItemId { get; set; }",
            "+ Kind { get; set; }",
        ],
        [
            "+ ApplyEffect(stats : PlayerStats) : void  {virtual}",
            "+ Clone() : Item  {virtual}",
        ],
        stereotype="«abstract Prototype»  not MonoBehaviour",
        abstract=True,
        fill=PEACH,
        edge=PEACH_EDGE,
        header_fill="#F8E6D0",
    )

    item_kind = enum_box(
        ax,
        160,
        95.2,
        28,
        "ItemKind",
        ["None", "WeaponUpgrade", "ArmorUpgrade", "Consumable", "Relic"],
    )

    loot_table = class_box(
        ax,
        23,
        64.8,
        40,
        "LootTable",
        [
            "− prototypes : Item[*]",
            "− dropRates : Map<RoomType, float>",
            "− weights : Map<RoomType, (string, int)[*]>",
            "+ Prototypes { get; }",
            "+ DropRates { get; }",
        ],
        [
            "+ RollDrop(roomType : RoomType) : Item",
        ],
        stereotype="«Prototype registry»  not MonoBehaviour",
    )

    weapon = class_box(
        ax,
        68,
        48.8,
        28,
        "WeaponUpgradeItem",
        [
            "− attackDelta : int  {5 | 8}",
            "+ AttackDelta { get; set; }",
        ],
        [
            "+ ApplyEffect(stats : PlayerStats) : void  {override}",
            "+ Clone() : Item  {override}",
        ],
    )
    armor = class_box(
        ax,
        102,
        47.6,
        30,
        "ArmorUpgradeItem",
        [
            "− defenseDelta : int  {3 | 5}",
            "− maxHealthDelta : int  {10 | 20}",
            "+ DefenseDelta { get; set; }",
            "+ MaxHealthDelta { get; set; }",
        ],
        [
            "+ ApplyEffect(stats : PlayerStats) : void  {override}",
            "+ Clone() : Item  {override}",
        ],
    )
    consumable = class_box(
        ax,
        138,
        46.4,
        31,
        "ConsumableItem",
        [
            "− consumableId : string",
            "− durationRemaining : float  {10.0}",
            "− attackDelta : int = 8",
            "+ ConsumableId { get; set; }",
            "+ DurationRemaining { get; set; }",
            "+ AttackDelta { get; set; }",
        ],
        [
            "+ ApplyEffect(stats : PlayerStats) : void  {override}",
            "+ Revert(stats : PlayerStats) : void",
            "+ Clone() : Item  {override}",
        ],
    )
    relic = class_box(
        ax,
        172,
        47.6,
        28,
        "RelicItem",
        [
            "− relicId : string",
            "− moveSpeedDelta : float = 0.15",
            "− critChanceDelta : int = 10",
            "+ RelicId { get; set; }",
            "+ MoveSpeedDelta { get; set; }",
            "+ CritChanceDelta { get; set; }",
        ],
        [
            "+ ApplyEffect(stats : PlayerStats) : void  {override}",
            "+ Clone() : Item  {override}",
        ],
    )

    loadout = class_box(
        ax,
        28,
        18.6,
        42,
        "Loadout",
        [
            "− items : Item[0..*]",
            "− consumableTimers : (string, float)[*]",
            "+ Items { get; }",
            "+ ConsumableTimers { get; }",
        ],
        [
            "+ ApplyItem(item : Item, stats : PlayerStats) : void",
            "+ CreateMemento() : ItemMemento",
            "+ SetMemento(m : ItemMemento) : void",
        ],
        stereotype="«Originator»  not MonoBehaviour",
        fill=PEACH,
        edge=PEACH_EDGE,
        header_fill="#F8E6D0",
    )

    memento = class_box(
        ax,
        118,
        19.4,
        44,
        "ItemMemento",
        [
            "− equippedItemIds : string[*]",
            "− consumableTimers : (string, float)[*]",
            "− statBaseline : PlayerStats",
            "− schemaVersion : int = 1",
        ],
        [
            "~ CopyFrom(loadout : Loadout) : void",
            "~ RestoreInto(loadout : Loadout) : void",
        ],
        stereotype="«Memento»  opaque lock-box",
    )

    left_rail = 2.6
    compose(
        ax,
        loot_system,
        loot_table,
        start=loot_system.bottom(),
        end=loot_table.top(),
        label="registry",
        lxy=(11.6, 78.8),
    )
    compose(
        ax,
        loot_system,
        loadout,
        start=loot_system.left(),
        end=loadout.left(),
        label="originator",
        lxy=(8.8, 40.6),
        via=[
            (left_rail, loot_system.cy),
            (left_rail, loadout.cy),
        ],
    )
    associate(
        ax,
        loot_system,
        memento,
        start=loot_system.right(),
        end=memento.top(),
        label="holds, never peeks",
        lxy=(58.4, 82.6),
        via=[
            (49.6, loot_system.cy),
            (49.6, 82.0),
            (memento.cx, 82.0),
        ],
    )
    associate(
        ax,
        loot_table,
        item,
        start=loot_table.right(),
        end=(item.x, item.cy - 2.0),
        label="clones prototype  (no new Subclass)",
        lxy=(56.8, 70.8),
        mult_b="1..*",
        mxy_b=(76.4, 74.6),
        via=[(49.6, loot_table.cy), (49.6, item.cy - 2.0)],
    )
    associate(ax, item, item_kind, label="kind", lxy=(132.4, 101.6))
    associate(
        ax,
        loadout,
        memento,
        label="writes / reads state",
        lxy=(72.4, 23.6),
        mult_b="1",
        mxy_b=(94.8, 16.4),
    )
    associate(
        ax,
        loadout,
        item,
        start=loadout.right(),
        end=(item.x, item.y),
        label="equipped",
        lxy=(58.8, 34.2),
        mult_b="0..*",
        via=[(49.6, loadout.cy), (49.6, 34.0), (item.x + 2.0, 34.0)],
    )

    bus_y = 66.8
    for child in (weapon, armor, consumable, relic):
        generalize(ax, child, item, bus_y=bus_y)

    note_box(
        ax,
        48.0,
        3.4,
        78,
        6.8,
        "Dynamic binding — ApplyEffect",
        [
            "ApplyItem holds an Item reference and calls ApplyEffect.",
            "Remove virtual / override → Item’s no-op runs; HUD stats freeze.",
            "NullItem is Kind = None on Item (not a fifth subclass).",
        ],
    )
    note_box(
        ax,
        132.0,
        3.4,
        78,
        6.8,
        "GoF + Unity",
        [
            "Prototype: LootTable.RollDrop clones a registered Item, never new.",
            "Memento: Loadout writes ItemMemento; LootSystem only shepherds it.",
            "Only LootSystem : MonoBehaviour (pickup + 10 s timer). Item.Clone stays plain C#.",
        ],
    )

    return save(fig, "class_diagram.png")


if __name__ == "__main__":
    path = draw_class_diagram()
    print("Wrote", path, path.stat().st_size)

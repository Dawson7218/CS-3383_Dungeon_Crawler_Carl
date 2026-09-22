#!/usr/bin/env python3
"""Gane–Sarson Diagram 4 — Feature 4 Upgrade & Loot zoom (Champion process 4)."""

from __future__ import annotations

import os
from pathlib import Path

os.environ.setdefault("MPLCONFIGDIR", "/tmp/mplconfig")

import matplotlib.pyplot as plt
from matplotlib.patches import FancyArrowPatch, FancyBboxPatch, Rectangle

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

    def top(self, inset=0.0, dx=0.0):
        return (self.cx + dx, self.y + self.h - inset)

    def bottom(self, inset=0.0, dx=0.0):
        return (self.cx + dx, self.y + inset)

    def left(self, inset=0.0, dy=0.0):
        return (self.x + inset, self.cy + dy)

    def right(self, inset=0.0, dy=0.0):
        return (self.x + self.w - inset, self.cy + dy)


def process(ax, cx, cy, w, h, pid, name, fill=WHITE, edge=NAVY):
    box = Box(cx, cy, w, h)
    x, y = box.x, box.y
    ax.add_patch(
        FancyBboxPatch(
            (x, y),
            w,
            h,
            boxstyle="round,pad=0.10,rounding_size=0.22",
            facecolor=fill,
            edgecolor=edge,
            linewidth=1.45,
            zorder=3,
        )
    )
    header = h * 0.36
    ax.plot([x + 0.7, x + w - 0.7], [y + h - header, y + h - header], color=edge, lw=1.15, zorder=4)
    text(ax, cx, y + h - header / 2, pid, size=9.4, weight="bold")
    text(ax, cx, y + (h - header) / 2 - 0.05, name, size=8.6)
    return box


def store(ax, cx, cy, w, h, did, name):
    """Gane–Sarson store: open on the right, ID band on the left."""
    box = Box(cx, cy, w, h)
    x, y = box.x, box.y
    ax.add_patch(Rectangle((x, y), w, h, facecolor=WHITE, edgecolor="none", linewidth=0, zorder=3))
    split = w * 0.22
    ax.plot([x, x + w], [y + h, y + h], color=NAVY, lw=1.55, zorder=4, solid_capstyle="butt")
    ax.plot([x, x + w], [y, y], color=NAVY, lw=1.55, zorder=4, solid_capstyle="butt")
    ax.plot([x, x], [y, y + h], color=NAVY, lw=1.55, zorder=4, solid_capstyle="butt")
    ax.plot([x + split, x + split], [y, y + h], color=NAVY, lw=1.55, zorder=4, solid_capstyle="butt")
    text(ax, x + split / 2, cy, did, size=8.8, weight="bold")
    text(ax, x + split + (w - split) / 2, cy, name, size=8.6)
    return box


def flow(ax, pts, label=None, lxy=None, lsize=7.4):
    xs, ys = [p[0] for p in pts], [p[1] for p in pts]
    ax.plot(xs, ys, color=NAVY, lw=1.3, zorder=2, solid_capstyle="round")
    ax.add_patch(
        FancyArrowPatch(
            pts[-2],
            pts[-1],
            arrowstyle="-|>",
            mutation_scale=12,
            linewidth=1.3,
            color=NAVY,
            zorder=2,
            shrinkA=0,
            shrinkB=0,
        )
    )
    if label and lxy:
        ax.text(
            lxy[0],
            lxy[1],
            label,
            fontsize=lsize,
            color=TEXT,
            ha="center",
            va="center",
            fontstyle="italic",
            zorder=6,
            clip_on=False,
            bbox={"facecolor": "white", "edgecolor": "none", "pad": 0.7, "alpha": 0.94},
        )


def note_box(ax, cx, cy, w, h, title, lines):
    x, y = cx - w / 2, cy - h / 2
    ax.add_patch(Rectangle((x, y), w, h, facecolor="#F7F4EE", edgecolor=NAVY, linewidth=1.15, zorder=3))
    fold = 1.3
    ax.add_patch(
        Rectangle((x + w - fold, y + h - fold), fold, fold, facecolor="#E8E0D4", edgecolor=NAVY, linewidth=0.9, zorder=4)
    )
    text(ax, x + 0.75, y + h - 1.25, title, size=7.1, weight="bold", ha="left")
    for i, line in enumerate(lines):
        text(ax, x + 0.75, y + h - 2.7 - i * 1.35, line, size=6.6, ha="left")


def draw_dfd4():
    fig, ax = plt.subplots(figsize=(16.8, 10.4))
    ax.set_xlim(0, 168)
    ax.set_ylim(-1, 104)
    ax.set_aspect("equal")
    ax.axis("off")
    fig.patch.set_facecolor("white")
    ax.set_facecolor("white")

    text(ax, 84, 102.4, "Diagram 4 — Upgrade & Loot", size=13.4, weight="bold", color=TITLE_C)
    text(
        ax,
        84,
        99.5,
        "Zoom of Champion process 4 only. White boxes are sibling features (sources / sinks). Peach boxes are Feature 4.",
        size=7.5,
        color=MUTED,
    )
    text(
        ax,
        8.4,
        96.7,
        "Notation:  numbered rounded rectangle = Gane–Sarson process     open-right = data store     every flow starts and ends on a box",
        size=6.6,
        ha="left",
        color=MUTED,
    )

    p2 = process(ax, 22, 82.4, 30, 12.2, "2", "Generate Dungeon Map")
    p3 = process(ax, 22, 56.2, 30, 12.2, "3", "Spawn Encounters")
    p7 = process(ax, 22, 22.0, 30, 12.2, "7", "Control Player & Combat")

    p41 = process(ax, 72, 82.4, 30, 13.0, "4.1", "Roll Loot Drop", fill=PEACH, edge=PEACH_EDGE)
    p42 = process(ax, 118, 82.4, 30, 13.0, "4.2", "Apply Effect", fill=PEACH, edge=PEACH_EDGE)
    p43 = process(ax, 118, 52.6, 30, 13.0, "4.3", "Revert Consumable", fill=PEACH, edge=PEACH_EDGE)
    p44 = process(ax, 118, 22.0, 30, 13.0, "4.4", "Snapshot Loadout", fill=PEACH, edge=PEACH_EDGE)

    d1 = store(ax, 72, 56.2, 30, 10.2, "D1", "Item Catalog")
    d2 = store(ax, 154, 22.0, 26, 10.2, "D2", "Player Loadout")

    # Dedicated vertical rails so sibling / Feature 4 flows do not share a shaft.
    rail_clear = 40.4
    rail_stats_in = 46.2
    rail_stats_out = 52.0
    rail_revert = 90.4

    # 2 / 3 / D1 → 4.1 → 4.2  — labels sit on the shaft they name
    flow(ax, [p2.right(), p41.left()], "room type", (47.0, p2.cy))
    flow(
        ax,
        [p3.right(), (rail_clear, p3.cy), (rail_clear, p41.cy - 3.4), p41.left(dy=-3.4)],
        "room cleared",
        (rail_clear, 68.8),
    )
    flow(ax, [d1.top(), p41.bottom()], "templates, weights", (d1.cx, 69.2))
    flow(ax, [p41.right(), p42.left()], "dropped item", (95.0, p41.cy))

    # PlayerStats rides above 4.1 / 4.2 so it does not cross D1 → 4.1.
    flow(
        ax,
        [
            p7.right(dy=2.6),
            (rail_stats_in, p7.cy + 2.6),
            (rail_stats_in, 91.6),
            (p42.cx, 91.6),
            p42.top(),
        ],
        "PlayerStats",
        (95.0, 91.6),
    )
    # Leave 4.2 on the left so this horizontal never crosses the 4.2 → 4.3 shaft.
    gap_x = (p41.x + p41.w + p42.x) / 2
    flow(
        ax,
        [
            p42.left(dy=-4.8),
            (gap_x, p42.cy - 4.8),
            (gap_x, 64.6),
            (rail_stats_out, 64.6),
            (rail_stats_out, p7.cy - 2.6),
            p7.right(dy=-2.6),
        ],
        "stat deltas",
        (rail_stats_out, 38.4),
    )
    flow(
        ax,
        [p43.left(dy=-2.4), (rail_revert, p43.cy - 2.4), (rail_revert, 12.2), (p7.cx, 12.2), p7.bottom()],
        "reverted stats",
        (rail_revert, 16.4),
    )

    # Feature 4 internals — one clean vertical for effect record; loadout goes around 4.3.
    flow(ax, [p42.bottom(), p43.top()], "effect record", (p42.cx, 67.5))
    rail_loadout_r = p42.x + p42.w + 4.4
    flow(
        ax,
        [
            p42.right(dy=-4.4),
            (rail_loadout_r, p42.cy - 4.4),
            (rail_loadout_r, p44.y + p44.h + 5.8),
            (p44.cx + 7.0, p44.y + p44.h + 5.8),
            p44.top(dx=7.0),
        ],
        "loadout delta",
        (rail_loadout_r, 48.0),
    )
    flow(ax, [p44.right(), d2.left()], "ItemSnapshot", (136.2, p44.cy - 3.5))
    flow(
        ax,
        [d2.top(), (d2.cx, p42.cy), p42.right()],
        "current loadout",
        (d2.cx, 54.0),
    )

    note_box(
        ax,
        72.0,
        3.0,
        78,
        5.6,
        "Feature 4 stores and scope",
        [
            "D1 and D2 belong to process 4. Processes 2, 3, and 7 are not exploded here.",
            "A duplicate-relic no-op is a valid output of 4.2, not a missing flow.",
        ],
    )
    note_box(
        ax,
        140.0,
        3.0,
        50,
        5.6,
        "Champion primitives",
        [
            "4.1 RollDrop   4.2 ApplyEffect",
            "4.3 Revert     4.4 Save / Restore",
        ],
    )

    return save(fig, "dfd4_upgrade_and_loot.png")


if __name__ == "__main__":
    path = draw_dfd4()
    print("Wrote", path, path.stat().st_size)

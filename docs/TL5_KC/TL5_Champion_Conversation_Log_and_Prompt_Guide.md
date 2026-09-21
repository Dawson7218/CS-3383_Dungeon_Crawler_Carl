# TL5 Champion Document — Conversation Log and Reusable Prompts

**Project:** Dungeon Crawler Carl  
**Developer:** Sahadeep Kc, TL5  
**Feature:** Feature 6 — Boss Encounter System  
**Log prepared:** September 21, 2026  
**Document reached:** `Dungeon_Crawler_Carl_TL5_Champion_Document.docx`, revised 23-page version  
**Status:** Accepted by the user as a working version; professor feedback and teammate comparison may require further revisions.

This is a summary of the important decisions and revisions in our conversation, not a verbatim transcript or a record of implementation hours. Quoted excerpts below come from the user's messages. Copy-ready prompts are edited templates for other developers to adapt; they are not presented as exact historical quotations.

## 1. Goal and starting information

The initial task was to use a sample Champion document as the structure for a feature-specific design document. The required content covered five areas:

1. Introduction: what the feature does and does not do.
2. Use case: actors, a numbered main path, and exceptions branching from specific steps.
3. Interface contract: typed, testable inputs, outputs, and events.
4. Gane–Sarson DFDs: a context view followed by feature decomposition, with valid inputs and outputs.
5. Timeline: work items and honest hour estimates that TL4 can track.

The supplied TL5 specification established three boss phases, the Chain of Responsibility pattern, polymorphic attack behavior, and these public interfaces:

```csharp
void StartEncounter();
event Action OnBossDefeated;
BossPhaseInfo GetCurrentPhase();
```

It also established the 52-hour effort baseline: Design 11, Coding 26, Testing 10, and Documentation 5. These were planning estimates, not hours spent in this conversation or evidence of completed coding.

## 2. Major stages and changes

| Stage | User direction | Change or decision | Why it mattered |
| --- | --- | --- | --- |
| Establish the structure | Use the sample and the five required parts. | Organized the feature around scope, behavior, contracts, data flow, and estimated work. | Created a design that a developer and reviewer could assess. |
| Correct use-case relationships | Replace solid dependency lines with dotted/dashed lines and include arrowheads; use the relationship reference. | Distinguished actor associations from UML include/extend relationships. | Arrow style and direction express different relationships. |
| Explain dynamic binding | Identify the classes, the role of MonoBehaviour, and the pattern class diagram. | Explained BossController, the BossPhase base class, and all three overrides; connected the design to Chain of Responsibility. | Made the programming mechanism explicit rather than assuming Unity supplied the required behavior. |
| Expand diagram scope | Include the whole team's features in context and Diagram 0. | Kept the detailed Champion scope on TL5 while giving the top-level DFDs a whole-game boundary. | Showed integration with teammates without treating internal features as external systems. |
| Tighten DFD rules | Use data packets rather than calls/events; number processes, show owners, balance levels, and derive stores from actual features. | Used labeled data flows and explained how public interfaces correspond to those data transfers. | Prevented a sequence diagram from being mislabeled as a DFD. |
| Confirm the intended deliverable | Imitate the supplied format, focus on TL5, and wait for “Go.” | Established the scope and reference format before generating the full version. | Reduced the chance of producing a whole-team Champion document instead of the user's own feature document. |
| Deepen the design | Include contracts, classes, dynamic binding, patterns, testing, dependencies, and estimates. | The working document uses ten sections to expand the five required areas into a detailed design and acceptance plan. | Gave the developer implementation guidance and reviewers concrete acceptance criteria. |
| Replace the basic sequence | Make Section 2.3 a full scenario. | Added participants, goal, trigger, preconditions, the full fight through victory, step-linked alternatives, and postconditions. | Covered what happens before, during, and after the normal fight, including failure paths. |
| Highlight decomposition | Highlight Feature 6 in Diagram 0 and the process expanded at each later level. | Added yellow highlights and captions tracing 6.0 → 6.2 → 6.2.3. | Let a reader follow the selected process through the hierarchy. |
| Clean formatting | Improve document formatting. | Improved numbered-step indentation, table alignment, title treatment, captions, and page layout; rendered and visually checked the revised document. | Made the result easier to read and share. |

## 3. Important technical decisions

### Scope and numbering

The entire game is Process 0 in the context diagram. Diagram 0 decomposes the game into its feature processes, with TL owners included in the labels. TL5's boss remains **Process 6.0**; process numbers follow feature numbers, not TL numbers. TL6 owns the player feature, and TL2 owns room/map design. Teammates should use the actual shared feature specification when assigning all remaining owners.

The highlight path is:

- Diagram 0: highlight **6.0 — Boss encounter**; expand it in Diagram 6.
- Diagram 6: highlight **6.2 — Resolve live phase**; expand it in Diagram 6.2.
- Diagram 6.2: highlight **6.2.3 — Commit phase snapshot**; specify it in the primitive process description, Section 4.4.1.

This does not mean every child process is highlighted. The highlight identifies the particular process being taken to the next level of detail.

### UML relationships and DFD flows are different

The early request for dotted lines concerned UML use-case dependencies. Include and extend relationships use dashed lines with open arrowheads. Actor associations use solid lines without arrowheads. An include arrow points toward the included use case; an extend arrow points from the extending use case to the base use case.

DFD arrows represent named data transfers. They should not be labeled as method calls, sequence steps, or execution commands. For example, `OnBossDefeated` is a published event, while **Defeat fact** describes the information represented by that notification in the DFD. The API remains documented in the interface contract and sequence diagram.

### Dynamic binding and the design pattern

`BossController` inherits from `MonoBehaviour` and connects the encounter to Unity. `BossPhase` is an ordinary abstract C# class, not a MonoBehaviour. Its virtual `ExecutePhase(BossController ctx)` method is overridden by:

- `Phase1_RangedBarrage`
- `Phase2_SummonMinions`
- `Phase3_Enrage`

The controller calls through a `BossPhase` reference, and C# selects the derived implementation at runtime. A method itself cannot inherit from MonoBehaviour. The explicit virtual/override design is the feature's dynamic-binding mechanism.

For Chain of Responsibility, BossController is the client, BossPhase is the handler abstraction, and the three concrete phases handle their health ranges or forward selection to a successor. A zero-health check precedes live phase selection.

An important clarification was that simply deleting `virtual` while leaving `override` causes a compilation error. A controlled static-dispatch demonstration also needs compatible method declarations; it can then show a base-reference call using the base attack behavior.

### Testable contracts and honest assumptions

The published TL5 API was preserved. Additional integration records and behavior choices were marked as proposed design details requiring team agreement. Health thresholds of 70% and 35%, with a 1,000-health test fixture, were proposed values rather than values supplied in the original feature requirements.

The document describes planned tests for thresholds, distinct attack patterns, phase skipping, cancellation, duplicate notifications, and integration. It does not claim that those tests have passed or that the game is implemented.

## 4. Most helpful original prompts

### A. Set concrete acceptance criteria at the start

> “Introduction —what your feature does, and what it does NOT do.”
>
> “Interface Contract — inputs / outputs / events, each typed and testable (not vague).”
>
> “Timeline — work items + honest hour estimates (TL4 tracks you against these).”

**Why it helped:** These requirements made scope boundaries, verifiable behavior, and realistic work estimates part of the initial task.

### B. Ask about the actual programming mechanism

> “Which classes will my dynamic binding be in?”
>
> “Is "FunctionNameHere" going to be a child of unity monobehavior as that would handle dynamic binding which we need to do manually?”
>
> “Which classes are involved in my design pattern? What do the class diagrams of each design pattern look like for my feature”

**Why it helped:** These questions exposed the distinction between Unity components, C# inheritance, runtime dispatch, and the GoF pattern.

### C. Define the DFD rules explicitly

> “The diagrams show **data flow, not control flow**. Every arrow is a **labeled data flow** (a named packet of data), never a method call, event, or sequence step.”
>
> “Balance the diagram: every flow that crosses the boundary in the context diagram must reappear here, entering or leaving the same agent.”

**Why it helped:** This was especially useful for correcting diagram meaning, not just appearance. It also made parent-child consistency reviewable.

### D. Separate personal scope from whole-team context

> “I need a champion document for my part covering the TL5 features but yes the context diagram and 0 should have the whole features included.”

**Why it helped:** This resolved the most important scope ambiguity: whose design the document explains versus how broadly the overview diagrams must look.

### E. Identify the precise final revision

> “2.3 Basic sequence for the boss fight - This should be a full scenario, not just the Basic Sequence.”
>
> “The Diagram 0 should have your feature (6?) highlighted somehow, and each level after that has to highlight the feature that is being taken down to the next stage”

**Why it helped:** Naming the section and describing the intended result allowed a focused revision. The first instruction expanded scenario coverage; the second created a visible decomposition trail.

## 5. Copy-ready prompts for teammates

Replace bracketed placeholders and attach the actual feature specification and sample before using these templates.

### Prompt 1 — Read the references and establish scope

```text
Read the attached Champion sample and the complete team feature specification.
My owner is [TL#], and my feature is [feature number and name].

Follow the sample's organization and formatting. My Champion document should
cover my feature in detail, but its Context Diagram and Diagram 0 must cover
the entire game and preserve the team's actual feature ownership.

Before drafting, summarize my scope, public interfaces, dependencies,
required sections, and diagram hierarchy. Identify missing information or
conflicts. Do not invent missing teammate interfaces or ownership assignments.
```

### Prompt 2 — Generate the Champion document

```text
Create my Champion document using the supplied sample and confirmed scope.
Include an introduction with exclusions, complete use-case scenarios,
typed and testable interface contracts, Gane–Sarson DFDs, and work items
with honest hour estimates. Preserve the published API signatures.

Explain my classes, dynamic binding, GoF pattern, GRASP principles,
dependencies, and acceptance tests. Mark proposed thresholds, adapters,
and other assumptions clearly. Do not claim planned tests have passed.
Use the reference's additional sections where appropriate.
```

### Prompt 3 — Turn a basic sequence into a complete scenario

```text
Expand Section [number/title] into a complete scenario. Include the goal,
actors, trigger, preconditions, numbered success path, alternatives and
exceptions, success postconditions, and failure/cancellation guarantees.

Link exceptions to specific steps, such as Step 3a. State whether each
branch resumes a step, ends the scenario, or leaves it suspended.
Carry the scenario through the final user-visible outcome. Keep it
consistent with the public contract and distinguish proposed behavior
from requirements already agreed by the team.
```

### Prompt 4 — Produce and balance the whole-team DFDs

```text
Create a Context Diagram and Diagram 0 in Gane–Sarson notation using the
attached whole-team specification. Context: one process numbered 0 with
the game name, external agents, and labeled boundary flows; no internal
processes or stores. Diagram 0: feature processes with numbers and TL owners,
the same external agents, justified data stores, and named data transfers.

Use squares for external agents, numbered rounded rectangles for processes,
and open-ended rectangles for stores. Labels must describe data packets,
not method calls, events, or sequence steps. Explain how cross-feature
interfaces carry those packets. Preserve feature numbering and ownership.
Check boundary balance and identify any process missing inputs or outputs.
Do not add persistence or external services absent from the specification.
```

### Prompt 5 — Highlight the decomposition path

```text
In Diagram 0, highlight my feature process [number/name] in pale yellow.
In each deeper DFD, highlight only the process being expanded at the next
level. Add a caption naming the highlighted process and its destination
diagram or primitive description. Keep numbering consistent across levels.
Do not change the underlying flows merely to accommodate highlighting.
```

### Prompt 6 — Explain dynamic binding and the pattern

```text
Using my feature specification, identify the base class, derived classes,
virtual or abstract methods, overrides, and the caller using a base reference.
Explain which class inherits from MonoBehaviour and which classes are ordinary
C# objects. Show the classes and relationships involved in the GoF pattern.

Explain what changes if dynamic binding is removed. Distinguish compilation
errors from a working static-dispatch demonstration. Preserve my published API
and label internal helper methods as design proposals where appropriate.
```

### Prompt 7 — Review formatting and internal consistency

```text
Clean up this document's headings, tables, numbered steps, captions,
spacing, and page breaks. Render it and visually inspect every page for
clipping, unreadable diagrams, awkward splits, and alignment problems.

Check that the scenario, contract, DFDs, classes, tests, and estimates agree.
Preserve the published API, ownership, and approved scope. Report the major
changes and any unresolved assumptions. Return the revised document.
```

## 6. Lessons to share with the team

- Supply both the sample and the actual feature specification. A sample establishes format; it does not establish your game's architecture.
- Ask for precise behavior and acceptance criteria instead of only asking for more detail.
- Keep UML, DFD, and sequence notation separate. Similar-looking arrows can have different meanings.
- Define the system boundary before drawing internal processes.
- Review the document's technical content as well as its visual presentation.
- Give targeted feedback using section numbers and desired outcomes.
- Keep proposed integration decisions visible so teammates can resolve them together.

## 7. Current endpoint and next review

The latest revision contains the expanded boss-fight scenario, a highlighted DFD decomposition path, and cleaned formatting. The user accepted it as a good working version pending professor feedback and teammate review.

The conversation also requested editable draw.io XML and a developer highlight map earlier in the process. This log records those requests but does not assert that a separate editable master was delivered with the latest Word revision. Verify available deliverables before promising that source file to teammates.

For the next revision, collect the professor's comments and the teammates' current specifications, identify the affected sections, and update related scenarios, interfaces, diagrams, and tests together. Keep the 52-hour planning baseline distinct from actual hours tracked by TL4.

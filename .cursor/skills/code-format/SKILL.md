---
name: code-format
description: >-
  Formats C# type bodies in the skill’s project root: adds XML /// documentation where missing, wraps members in
  ordered #region blocks (Fields, Events, Ctors, Utils, Methods, Properties, Operators, Nested), places explicit
  interface implementations in trailing #region {Interface} Members blocks, and sorts members alphabetically inside
  each region. Use when the user asks for code-format, regions, member ordering, or XML summaries within that
  subfolder only (not sibling monorepo projects).
---

# Code format (XML docs + regions + ordering)

## Project root boundary (mandatory)

Same rule as before: **only** edit files under **`<ProjectRoot>`**, where this file is `.../<ProjectRoot>/.cursor/skills/code-format/SKILL.md`. Do not scan or change files outside `ProjectRoot`.

Excluded under `ProjectRoot`: `bin/`, `obj/`, generated files, `*.Designer.cs`, third-party verbatim copies.

## Goals (in order)

1. **`///` XML documentation**: `<summary>` and related tags for members missing or empty docs (see rules below).
2. **`#region` grouping**: Use the region names and order below; omit regions that would be empty.
3. **Alphabetical order**: Within each region, order members **A–Z** by **member name** (case-insensitive, invariant culture), unless the user specifies a different collation.

## Region blocks (syntax)

Use this shape (blank line after `#region` is optional; match the project’s existing style if already present):

```csharp
#region Methods

public override string ToString() => "";

#endregion Methods
```

- **Standard** region names: **exact** `Fields`, `Events`, `Ctors`, `Utils`, `Methods`, `Properties`, `Operators`, `Nested` (Pascal case after `#region` / `#endregion`).
- **Standard region order** (top to bottom): **Fields → Events → Ctors → Utils → Methods → Properties → Operators → Nested**.
- Include **only** standard regions that contain at least one member after classification.
- **After** all standard regions that apply, append **explicit interface implementation** regions (see next section). Nothing follows them except the type’s closing `}`.

## Explicit interface implementation regions (mandatory when present)

Use this pattern for members declared as **`ReturnType IInterface<...>.MemberName`** (methods), **`T IInterface<...>.Property`**, or explicit **`event`** implementations:

```csharp
#region IRequestHandler Members

Task<Unit> IRequestHandler<TCommand, Unit>.HandleAsync(TCommand command, CancellationToken cancellationToken)
{
    return HandleAsync(command, cancellationToken);
}

#endregion IRequestHandler Members
```

Rules:

1. **Do not** place these members in **Utils**, **Methods**, or **Properties**; they live only in their **interface** region.
2. **Region label**: `#region {InterfaceName} Members` / `#endregion {InterfaceName} Members`, where **`InterfaceName`** is the **simple** interface type name **without** the generic arity list (e.g. `IRequestHandler<TCommand, Unit>` → **`IRequestHandler`**; `System.IDisposable` → **`IDisposable`**).
3. **Position**: **last** in the type body—after **Nested** (or after whichever is the last **non-empty** standard region if **Nested** is omitted).
4. **Several interfaces**: use **one region per distinct `InterfaceName`**. Order those regions **alphabetically** by `InterfaceName` (e.g. `IDisposable` before `IRequestHandler`).
5. **Several members** for the same interface: keep them in **one** region; sort **alphabetically** by member name inside the region.

## Member classification

| Region | Contents |
|--------|----------|
| **Fields** | `private` instance/static **fields** and **`const`** (including `private const`). |
| **Events** | **Event**-like surface: `event` declarations; **`public`** delegates / multicast types that are clearly intended as event callbacks (`Action`/`Func`/`EventHandler` fields used as events, custom `delegate` types when `public`). If unsure, prefer **`event`** keyword here. |
| **Ctors** | **Constructors** only (instance and `static` `.cctor`). **Finalizers** (`~Type()`) are not constructors → **Utils**. |
| **Utils** | Members that are **not** `public` and are **not** explicit interface implementations: `private` / `internal` / `protected` / `protected internal` **methods**. |
| **Methods** | **`public` methods** (including `public override`, `public virtual`, **public** interface implementations). |
| **Properties** | **Properties** and **indexers** (`this[...]`) that are **not** explicit interface implementations. |
| **Operators** | **User-defined operators** (`operator +`, `implicit`/`explicit` conversions). |
| **Nested** | **Nested** `class`, `struct`, `record`, `interface`, `enum` declarations. |

### Everything else

- **`public` / `protected` / `internal` `const` or `static readonly` fields** (not `private`): place in **Properties** if they read like API constants; otherwise **Methods** only when they are clearly ancillary to behavior—prefer **Properties** for stable named values exposed to callers.
- **Static local / file-level** members: normal C# has no file-level in classic classes; **file-scoped types** are out of scope for inner regions.
- **Records** with primary constructor: **do not** tear apart the primary constructor line from the type declaration; apply regions only to **additional** members in the type body.
- **Interfaces**: regions optional; if used, only regions that exist and **no** forced **Fields**/**Ctors** unless applicable.

## XML documentation (unchanged intent)

- Prioritize **`public`** / **`protected`**; **`internal`** when central; **`private`** only if unusually complex or requested.
- Do not replace good existing `///` text; fill gaps and placeholders only.
- Doc text **English** unless user/workspace asks otherwise.
- **No** drive-by refactors beyond ordering, regions, and docs.

## Work order

1. Confirm paths ⊆ `ProjectRoot`.
2. Classify each member → standard region **or** explicit-interface region (by interface simple name).
3. Sort within each region alphabetically by name.
4. Emit standard regions in the fixed order; **then** emit explicit-interface regions (alphabetically by interface name).
5. Add missing `///` where appropriate.
6. Verify compile-oriented sanity (no duplicate `#endregion` labels, no nested region chaos).

## Conflicts

If alphabetical order would break a **required** semantic order (rare: `#if` interleaving), keep **alphabetical** within logically contiguous members and document the exception in a single-line comment only if the user already uses that pattern.

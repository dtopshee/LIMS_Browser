# Legislation Time Machine (LIMS PIT-Diff Engine)

A high-performance Blazor WebAssembly tool designed to visualize the evolution of the **Criminal Code of Canada** and other federal legislation. 

## The Core Problem
Federal legislation is published as **Point-in-Time (PIT) XML**. While these files provide a snapshot of the law on any given date, tracking the *transformation* of specific sections over decades is difficult due to structural reorganization (e.g., a Section renumbering into multiple Subsections).

## Technical Architecture
This application leverages the **`lims:fid` (Feature ID)**—an immutable anchor attribute—to track legislative elements across different snapshots, regardless of label changes or structural nesting.

### Key Features:
- **LIMS Schema Aware:** Specifically handles the `Section` -> `Subsection` transformation logic.
- **Side-by-Side Synchronized Scroll:** Compare two dates (e.g., 2005 vs 2026) with semantic highlighting.
- **Zero-Footprint SaaS:** Built as a client-side Blazor WASM app; no database or server required.

## The "Subsection Split" Logic
The engine monitors the `LegislativeNode` for changes in child counts. When the formula *"Section X is renumbered as subsection X(1) and amended by adding..."* occurs, the app identifies the persistent `fid` on the parent container to maintain the historical thread.

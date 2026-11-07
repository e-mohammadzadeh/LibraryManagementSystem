# Library Management System (LMS)

A console-based Library Management System built with **C#** and **.NET**, following clean architecture principles (Domain, Application, Infrastructure, Presentation).

This repository contains a complete **Version 1** (in-memory) implementation and is the home for **Version 2** work (database persistence).

---

## Overview

The system simulates real library operations: catalog management, membership, borrowing, returns, fines, authentication, role-based permissions, and audit-style history.

| Area | Features |
|------|----------|
| **Catalog** | Books, authors, translators (many-to-many) |
| **Users** | Members, librarians, admins · membership dates · roles |
| **Loans** | Borrow, return, renew · overdue tracking · loan history events |
| **Fines** | Geometric fine calculation · pay / waive · borrow blocks |
| **Security** | Email + password login · password hashing · session |
| **Authorization** | Permission-based access (not only role checks in UI) |
| **UX** | Console menus · table printers · search & sort |

---

## Version status

| Version | Status | Description |
|---------|--------|-------------|
| **v1.0.0** | Completed | Full in-memory LMS (tagged release) |
| **v2** | In progress | Database persistence (EF Core) |

- **V1 tag:** `v1.0.0` — stable in-memory version  
- **V2 branch:** active development toward SQL-backed repositories  

---

## Architecture

```text
Presentation  →  Console menus, printers, input helpers
Application   →  Services, DTOs, authorization, audit API
Domain        →  Entities, enums, repository interfaces
Infrastructure→  In-memory repos (v1) · EF Core / SQL (v2)
```

---

## Requirements

.NET SDK (version used by the solution)
Visual Studio 2022/2026 or VS Code
Windows recommended for console UX (box-drawing tables)



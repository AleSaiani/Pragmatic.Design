Pragmatic.Design is an initiative aimed at creating a design system (or pattern, boilerplate, template) that is tailored to expedite the development of Zupit-style software projects. The main objective is to offer a highly reusable initial structure and a set of coding conventions that optimize development efficiency and code maintenance while maintaining high software quality.

The project is built on robust, flexible, and widely adopted technologies: Asp.NET Core 7, Angular (ngrx + nx), and PrimeNG (https://primeng.org/) but in the near future extended to other stacks (like Blazor, Vue or others).

### Principles

Pragmatic.Design adheres to the following fundamental principles:

- Pragmatism: Decisions are made based on a cost-benefit analysis, striving to maximize effectiveness while maintaining quality.
- Feature Release Over Perfection: The goal is to reach a completed and working functionality rather than perfection. In other words, "done is better than perfect".
- Promote Reusability and Productivity: We don't want to reinvent the wheel. Where suitable solutions already exist, we will use them to maximize efficiency.
- Proximity Principle: Facilitating code discoverability and reducing complexity by grouping related code elements closely together, transforming traditional project layering into an approach based on classes, folders, or namespaces. This enhances developer productivity and simplifies system architecture navigation.

The project focuses on a series of common features that frequently reoccur in software projects, such as lists, details, modals, login, user profile, application startup, logging, deployment on Azure, notifications, etc. The goal is to define a standard approach to handling these aspects, thereby enhancing productivity, quality, and consistency.

We acknowledge that perfection can't be achieved in one iteration. Therefore, the project is designed to evolve over time, with continual feedback allowing for consistent improvements to the design system.

While we aim to avoid defining too many "micro" details, we recognize that some parts can be fully reused (login, startup, folder structure, etc.). In these cases, we design with the objective of creating reusable nuget/npm packages.

In an ideal world, we want to be able to port a feature from one software A to another software B if it's "roughly" the same feature. This vision guides the development of Pragmatic.Design.

### Pragmatism 

In Pragmatic.Design, the principle of pragmatism takes center stage. We aspire to make decisions based on a thorough cost-benefit analysis, emphasizing practical, efficient solutions that offer the highest return on investment. Our approach acknowledges the reality of software development: time and resources are often limited, and we must prioritize effectiveness and feasibility. By taking a pragmatic approach, we aim to maximize the value and productivity of our development process, focusing on solutions that work well in practice, rather than striving for theoretical perfection. This principle empowers us to create practical, robust, and efficient software.

### Feature Release Over Perfection

At the heart of Pragmatic.Design lies the principle of prioritizing feature release over perfection. In the world of software development, we acknowledge that 'done is better than perfect'. Striving for perfection can often lead to unnecessary delays and over-complication. Instead, our focus is on delivering functioning, high-quality features that bring immediate value to users. By balancing the need for quality with the importance of timely delivery, we ensure a steady, beneficial output, making continual progress and improvements. This principle encourages us to maintain momentum, celebrate each achievement, and constantly deliver value to our users.

### Promoting Reusability and Productivity

Promoting reusability and productivity is another cornerstone principle of Pragmatic.Design. We recognize that reinventing the wheel is inefficient and often unnecessary. Instead, we advocate for the reuse of existing, proven solutions wherever possible, optimizing productivity and reducing development time. By creating reusable components and making full use of existing libraries, tools, and frameworks, we can focus our efforts on creating unique features and improving the overall system. This principle guides us to be smarter in our development process, maximizing productivity, and emphasizing the efficient use of resources.

### Proximity Principle
The proximity principle is an important concept that we would like to integrate into Pragmatic.Design. It aims to facilitate code discoverability for developers, making the related code pieces more reachable by locating them closer to each other.

In many Domain-Driven Design (DDD) implementations in .NET, the codebase is usually divided into multiple projects for various divisions, such as domain, application, etc. While this may provide clear separation of concerns, it can lead to a fragmented codebase, making navigation and understanding the system architecture more complex than it needs to be.

In contrast, we propose a more pragmatic approach. For instance, an entity is a class that contains static nested classes responsible for persistence, specifications, and more. This is still based on CQRS (Command Query Responsibility Segregation) and domain events but reorganized following the proximity principle.

Similarly, a Query is a class that internally defines classes responsible for validation, FastEndpoints, Mediator's QueryHandlers, etc. This approach allows developers to see at a glance all the elements related to a specific Query or Entity, without the need to navigate through multiple projects or files.

This decision is motivated by our commitment to make a set of fixed assumptions (e.g., we use Entity Framework and we won't change it) that allow us to simplify the typical layer division carried out with projects into different classes, folders, or namespaces. This approach has the advantage of improving the code discoverability, thus enhancing the productivity and the quality of life of the developers.

This proximity principle, along with the original project goals and proposed features for the minimum viable version of Pragmatic.Design, embodies our dedication to fostering a pragmatic, efficient, and productive software development experience.


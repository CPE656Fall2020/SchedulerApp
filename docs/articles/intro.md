# System Overview

## Build Tools
The Power Scheduling System is developed in C#/WPF, targetting .NET Framework 4.7. The following environment is recommended for development:
- Microsoft Visual Studio 2019 [Community Edition or better]
- Git for Windows
  - *The binaries are automatically versioned from Git tag number, without system Git installed, the compilation process will fail.*
- \[Recommended] Microsoft Excel for use in data collection.
- \[Recommended] A text editor for writing JSON files
- \[Recommended] DB Browser for SQLite, if modifications are being made to the embedded database system.

## Hosting
All project components are hosted on GitHub. This documentation is automatically built from the documentation markdown hosted in the `/docs` folder, and combined with the API Reference manual generated automatically from the embedded xmldoc in the code. This documentation site (that you've found) is hosted with GitHub Pages, and is automatically deployed everytime a commit or Pull Request is merged into `master`.

## System Design
The Power Scheduling Application consists of three main components, each in its own C# Project File.
- **SchedulerGUI** is the main user-interface for the application, and houses the scheduling logic, along with user features. See the **Scheduler GUI Overview** article for details about this component.
- **SchedulerImportTools** is a command-line application and series of APIs that allow for importing experimental power profile data from Excel worksheets. This tool is usable stand-alone for bulk imports, and is incorporated into the GUI. See the **Import Tools Overview** article for additional details.
- **SchedulerDatabase** is the embedded database component of the application. This project houses all the database abstraction functionality, and programatically describes the schema of the database. This component provides database services to all other aspects of the system. See the **Database Components** article for more details.

## MVVM Paradigm
MVVM, or Model-View-ViewModel is the predominant pattern for WPF/C# UI development. MVVM is somewhat analogous to the common MVC (Model-View-Controller) pattern, although Controllers and ViewModels differ in some meaningful ways. 

- **Models** are simple classes, with little to no logic added, that describe the "shape" of the data being processed.
- **Views** represent the user-interface, and are done purely from a design and user-interaction perspective. In WPF, UIs are designed using XAML (eXtensible Application Markup Lanugage, an XML derivative), a declarative markup language. These XAML Views define the overall appearance/style and structure of the UI, but contain no data. Data is loaded from Models using dynamic bindings at runtime.
- **ViewModels** are the magic component that houses the translation and processing logic needed to create and process **Models** and expose them to **Views**
  - In MVVM, each View has a ViewModel.
  - WPF also defines "code-behind", which is the `MyView.xaml.cs` file, which allows for C# code that directly interacts with the View. This substantially breaks the MVVM paradigm of loose binding between C# logic code and displayed XAML UIs, and should only be used if absolutely necessary. All application logic code should be stored in `MyViewModel.cs` instead, and bound dynamically.

We are using "View-Model first" navigation, which is a style of MVVM that dictates that all navigation is handled with ViewModels, by ViewModels. This allows for ViewModels to never directly reference any View in code, enforcing strong seperation of concerns. Of course, these Views and ViewModels are 1-to-1 and have to be dynamically bound at runtime, which is handled by the WPF DataContext system. However, thse mapping must be defined in XAML once, in the  `Styles\ViewModelTemplates.xaml` file. This is used by the WPF system to initialize global Data Templates, so whenever a ViewModel requests navigation or display of a new ViewModel and associated data, the display system can implicitly create the correct view.

For a good outside reference on ViewModel First MVVM, see [Model-View-ViewModel-MVVM-Explained](https://www.wintellect.com/model-view-viewmodel-mvvm-explained/).

We are using the MVVMLight toolkit to aid in reducing MVVM boilerplate code. MVVMLight makes the property changed notifications substantially easier, and provides ready-to-use implementations of common requirements like `RelayCommand`. MVVMLight also provides basic dependency-injection functionality, which is used for some top-level ViewModel service registrations. Make good use of MVVMLight - it will make the code substantially easier and quicker to develop!

## Code Styling
In order to maintain high code quality, a uniform style is enforced on all C# code. To aid in this, the StyleCop.Analyzers NuGet package is used for compile-time source code validation. In large, the default style rules are used with only minor exceptions. When code quality problems are detected, Warning messages are emitted during the compilation process, and should be corrected. As a convenience, we do not treat Warnings as Errors that would half the compilation process, but all warnings should be resolved or dismissed prior to committing the code. Some of the major style points are:
- All public elements must be fully XMLdoc'ed. This includes ```protected```. The tool does not require xmldoc for ```private``` members, since that's a low-level implementation detail, however, they can be freely added if beneficial.
- Naming, spacing, and capitalization standards
- Ordering (Fields, Properties, and Methods, in that order, each sorted within by `public`, `protected`, `private`).
- Inclusion of this `this` prefix for class-scoped references.
- See StyleCop.Analyzers on NuGet or GitHub for full details.
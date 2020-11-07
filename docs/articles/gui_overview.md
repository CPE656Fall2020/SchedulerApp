# Scheduler GUI Overview
The Scheduler GUI project contains the bulk of the application source code. The following responsibilities are realized by this code:
- User input for orbital pass parameters
- User input for system parameters (battery and solar)
- User input for available devices (from SchedulerDatabase)
- Scheduling Capability for computing optmized device profiles for required tasks
- On screen comparison of available device profiles
- On screen display of inputted parameters
- On screen display of optimized profiles
- Report generation capabilities.

This article will summarize the high-level responsibilities of each major namespace or functional component of the application. The Api Documentation tab of this resouce can provide a great wealth of component-level design details regarding the implementation of the Scheduler GUI, and should be consulted for further details.

## WPF/MVVM Boilerplate Components
### Converters
In WPF, ValueConverters are an essential component used to translate one data type into another. ValueConverters are supported in bindings, and allow for reformatting and manipulating data from one portion of the ViewModel to be displayed in various locations of the View. Sample use cases include the common `NullToVisibilityConverter` and `BoolToVisibilityConverter` (the latter of which is common enough that it ships natively with .NET). These converters each convert from Object and Boolean types that satisfy a condition into a WPF Visibility level, and can be used to show or hide certain UI controls based on the state of data in the ViewModel and Model. Other examples of converters include complex string formatters to handle a variety of data types specific to the Scheduler application.

### Custom Controls
Custom Controls houses custom-developed and extended controls. These are similar to native WPF controls from the `System.Windows.` namespace.

## MVVM Specific Components
### Models
The Models namespace contains POCO classes that directly represent real-world data processed by the Scheduler Application. This includes things such as the Orbital Pass Parameters, Simulated Battery Characteristics, Simulated Solar Panel Parameters, and Encryption options. 

### Views
Views houses the XAML-based components that form the visible user interface. As these are MVVM-focused Views, they contain little to no traditional code-behind in the corresponding `View.xaml.cs` file. All data is loaded into the Views through XAML Data Binding, and is sourced from the matched ViewModel.

Examples of high-level views include the main window, the plot tool dialog, the about dialog, and the data import dialog.

### Control Views
Control Views are MVVM-based controls used to represent complex data. They are functionally no different than the elements housed in the Views namespace, and the distinction is purely application-specific. Our Control Views are intended to be used as much smaller scoped views that are composited into larger View (in the main Views namespace). 

Examples of control-level views include editor controls, popup dialog frames, graph controls, and custom list controls.

### View-Models
ViewModels house the bulk of the application processing (or "business") logic. A View-Model exists and for each and every View.

MVVMLight is used to simplify the implementation of ViewModels. As such, all VMs inherit from the `ViewModelBase` class. This base class also assists in raising the `IPropertyNotifyChangedEvent` triggers, which are used to efficiently update only portions of the View that are bound to data that is changing. This improves application performance since only small portions of the view have to be recomputed at a time as data is changed. 

MainViewModel is the entry point for the application, and contains the majority the scheduling flow-control logic.

## Application Specific Components

### Solver Components
The Solver namespace contains all algorithmic and data-structure components used to solve the device scheduling problem.

### Reporting Components
The Reporting namespace contains helper methods and the main report generation code for creating exportable reports. This functionality is based around WPF's `FlowDocument` model, and allows for the report to be fluidly generated at runtime. WPF Native XPS functionality, and Microsoft Print to PDF Driver are used for exporting the reports into a user-readable format for distribution.

### Settings
The Scheduler Application contains a robust user settings system, although a majority of the settings functionality currently remains unutilized. At startup, a user's local settings file is read from `settings.json` by the `SettingsManager`, and the processed parameters are made available throughout the application. And application component, via `SettingsManager`, can read and write values to the settings file. At the current time, only the location of the `data.db` file is stored in settings, although this is fully extensible and designed to accomidate future application growth.

### Services
Services is a catch-all namespace for universal components that perform low-level, application specific tasks. 

The `ViewModelLocator` service is used to provide the initial binding between the `MainWindow` view, and the `MainWindowViewModel` VM at application initialization.
# Deliveries by drones<br />

### .NET project written in C#, designing a drones-based delivery system management including admin and customer interface in WPF. <br />Built according to the 3-Tire Architecture model combining features such as simulator based on multithreading, XAML files, and Design Patterns: Singleton, Observer, Simple Factory, MVVM, Design by contract.


<br /><br /><br />**Simulator mode:**  all updates between the windows are performed by the MVVM architecture


https://user-images.githubusercontent.com/80857560/188131128-d8a30dc4-9173-4746-bcc5-9ede7fe926e9.mp4

<br /><br /><br />
**Manual mode:**


https://user-images.githubusercontent.com/80857560/188137398-73ee55ad-fd82-4941-a256-320f75475e36.mp4

<br /><br /><br />
**User interface:**


https://user-images.githubusercontent.com/80857560/188139738-4d744092-1e03-45e3-86e8-3f31ae25bc0f.mp4


<br /><br /><br />
## Design Patterns: 
#### Links to the implementation of the Design Patterns in the code (one example from the code for each one) <br />


<br />**Singleton:** <br />
https://github.com/Haiku54/Deliveries-by-drones/blob/main/BL/BLDrone.cs#:~:text=static%20readonly%20IBL,%3D%3E%20instance%3B%20%7D

<br />**Observer:**  <br />
https://github.com/Haiku54/Deliveries-by-drones/blob/main/PL/PO/ViewModel.cs#:~:text=public%20static%20ObservableCollection,.PackageToList%3E()%3B

<br />**Simple Factory:**<br />
https://github.com/Haiku54/Deliveries-by-drones/blob/main/DAL/DalFactory.cs#:~:text=public%20static%20class,%2C%20type)%3B

<br />**MVVM:**<br />
https://github.com/Haiku54/Deliveries-by-drones/blob/main/PL/PO/ViewModel.cs#:~:text=public%20class%20ViewModel%20%20//The%20class%20is%20responsible%20for%20updating%20all%20the%20data%20in%20the%20display

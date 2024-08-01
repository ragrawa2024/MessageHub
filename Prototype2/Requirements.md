## Objective

Design and implement a message-based system that leverages multiple concurrent collections, priority execution to manage concurrent operations effectively. 
The system will consist of a central component and multiple dependent components that communicate in real-time, demonstrating robust command-response handling and continuous data streaming capabilities.

## System Requirements

### Component Initialization
- Central Component: Acts as the core of the system, managing initialization and coordination of all dependent components.
- Dependent Components: A configurable number (N minimum 3) components that initialize in relation to the central component and communicate with Central Component and each other.

### Communication and Event Handling
- Real-time Communication: Dependent components must communicate with each other in real-time.
- Event Reception: The central component must receive and process events and messages from each dependent component efficiently.
- On Premise: All components should communicate with each other without access to internet.
Assume all components run on the same machine.

### Storage Layer
- Data Management: Implement a storage abstraction layer so that the solution is deployment agnostic.

### Command and Response Mechanism
- Round-trip Commands: The system should support a command and response mechanism, allowing commands to be sent from the central component to the dependent ones and vice versa.
- Continuous Data Stream: Provide an option for continuous streaming of data, which can be enabled or disabled as needed.

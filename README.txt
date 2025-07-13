Performance Testing Project with C# and NBomber

Project: AE-Automation-.NET-NBomber
Repository: https://github.com/Zstefanov/AE-Automation-.NET-NBomber

Description:
-------------
This project contains performance and load tests built using NBomber, a powerful and flexible .NET library for load testing. The tests are written in C# and designed to simulate real-world scenarios to evaluate system reliability, throughput, and responsiveness.
DATA used for accounts is not hidden, due to the nature of the project and its nature as a performance testing tool. It is intended for demonstration purposes only.

Features:
----------
- Easy-to-run console application
- Customizable NBomber scenarios and steps
- Flexible configuration for test duration, load simulation, and reporting

Getting Started:
-----------------
1. Clone this repository:
   git clone https://github.com/Zstefanov/AE-Automation-.NET-NBomber.git

2. Navigate to the project directory and install dependencies:
   dotnet restore

3. Run the performance tests(uncomment scenarios from the program.cs file to run different load tests):
   dotnet run

Requirements:
--------------
- .NET 9.0
- NBomber NuGet package (already included in the project)

Coverage:
--------------
This project includes comprehensive performance tests for the Automation Exercise website, covering various load scenarios such as:	
Login, Registration, Product Browsing, Cart Operations, Checkout, etc. There are positive and negative scenarios.


CI/CD:
--------------
- This project is integrated with GitHub Actions for CI/CD.
- This project is deployed to Jenkins locally via the GitHub repository.

References:
------------
- NBomber Documentation: https://nbomber.com/docs/getting-started/
- Project Repository: https://github.com/Zstefanov/AE-Automation-.NET-NBomber
- Website Tested: https://automationexercise.com

Author:
--------
Zs73fnv
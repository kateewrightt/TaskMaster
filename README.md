# Project Solve
Efficient Project Management Through Intelligent Task Sequencing

## Overview
ProjectSolve is an efficient software solution designed to transform project execution by managing task dependencies with ease. This system leverages data structures and algorithms to seamlessly organize tasks, ensuring they are executed in a logical and timely manner. From reading and storing task information to calculating the earliest task commencement times, ProjectSolve offers a user-friendly interface that streamlines project planning, reduces timelines, and maximizes resource utilization.

## How to Use

1.  Clone the repository to your local machine.
2.  Navigate to the project directory.
3.  Launch the ProjectSolve application.
4.  Choose from the following options:
-   **Import Tasks:** Provide a valid path to the file containing tasks to import them.
-   **Add New Task:** Input the task's name, time cost, and any dependencies (leave blank if none).
-   **Remove a Task:** Input the task's name to remove it from the list.
-   **Edit a Task:** Perform editing on a specific task.
-   **Save Task File:** Save the list of tasks to a file.
-   **Find a Sequence of Tasks:** Find a sequence of tasks based on their dependencies and save them to a file.
-   **Find Commencement Times:** Calculate and save the earliest commencement times of tasks.
-   **Exit:** End the program.

## Task Information Format
Each line in the text file corresponds to one task and follows the format:

***TaskID, TimeNeeded, Dependency1, Dependency2, ...***

- **TaskID:** A unique identifier (string) for the task, e.g., "T1," "T2," "T3," etc.
- **TimeNeeded:** A positive integer representing the time required to complete the task.
- **Dependencies:** A list of task IDs (separated by commas) on which the task depends. This ensures tasks are executed only after their dependencies are completed.

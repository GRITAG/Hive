# Queue Actions

* Add Task to Queue -
Adds a task tot the Queue

* Pull Task from Queue -
Sets the task to an active / assigned state and returns that task

* Peak at Next Task - 
looks at the next task to be pulled with out making the task active

* Pull all Tasks - 
Sets all currently queued tasks to an active / assigned state and returns a task list

* Peak all Tasks - 
looks at all tasks to be pulled with out making the tasks active

* Complete Task - 
moves the task to the complete table / store there by removing it from the queue

* Remove Task - 
removes the task from the queue

* Requeue Task -
requeuse a task by switching it state to inactive and deassigning it from a drone
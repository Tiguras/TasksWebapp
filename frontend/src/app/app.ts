import { Component, inject, OnInit, signal } from '@angular/core';
import { TaskItem, TaskService, TaskStatus, CreateTaskRequest } from './todo.service';
import { CdkDrag, CdkDragDrop, CdkDropList, transferArrayItem, moveItemInArray } from '@angular/cdk/drag-drop';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule, CdkDrag, CdkDropList, DatePipe],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private taskService = inject(TaskService);
  private toastr = inject(ToastrService);

  newTitle = '';
  newDescription = '';
  newDueDate = '';
  adding = false;
  pending = signal<TaskItem[]>([]);
  inProgress = signal<TaskItem[]>([]);
  completed = signal<TaskItem[]>([]);

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.taskService.getAll().subscribe(tasks => {
      this.pending.set(tasks.filter(t => t.status === 'Pending'));
      this.inProgress.set(tasks.filter(t => t.status === 'InProgress'));
      this.completed.set(tasks.filter(t => t.status === 'Completed'));
    });
  }

  addTask() {
    const title = this.newTitle.trim();
    const dueDate = this.newDueDate;
    if (!title || !dueDate || this.adding) return;

    const today = new Date().toISOString().split('T')[0];
    if (dueDate < today) {
      this.toastr.error('Due date cannot be in the past', 'Invalid Date');
      return;
    }

    this.adding = true;
    const req: CreateTaskRequest = {
      title,
      status: 'Pending',
      description: this.newDescription.trim() || undefined,
      dueDate,
    };
    this.newTitle = '';
    this.newDescription = '';
    this.newDueDate = '';
    this.taskService.create(req).subscribe({
      next: task => this.pending.update(list => [...list, task]),
      complete: () => this.adding = false,
    });
  }

  deleteTask(task: TaskItem, status: TaskStatus) {
    this.taskService.delete(task.id).subscribe(() => {
      if (status === 'Pending') this.pending.update(list => list.filter(t => t.id !== task.id));
      else if (status === 'InProgress') this.inProgress.update(list => list.filter(t => t.id !== task.id));
      else this.completed.update(list => list.filter(t => t.id !== task.id));
    });
  }

  drop(event: CdkDragDrop<TaskItem[]>, newStatus: TaskStatus) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
      const task = event.container.data[event.currentIndex];
      task.status = newStatus;
      this.taskService.updateStatus(task.id, newStatus).subscribe();
    }
  }
}

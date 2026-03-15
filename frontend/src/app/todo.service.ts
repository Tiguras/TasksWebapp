import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export type TaskStatus = 'Pending' | 'InProgress' | 'Completed';

export interface TaskItem {
  id: number;
  title: string;
  description: string | null;
  status: TaskStatus;
  dueDate: string | null;
  createdAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  status: TaskStatus;
  dueDate?: string;
}

@Injectable({ providedIn: 'root' })
export class TaskService {
  private http = inject(HttpClient);
  private url = '/api/tasks';

  getAll() {
    return this.http.get<TaskItem[]>(this.url);
  }

  get(id: number) {
    return this.http.get<TaskItem>(`${this.url}/${id}`);
  }

  create(task: CreateTaskRequest) {
    return this.http.post<TaskItem>(this.url, task);
  }

  update(item: TaskItem) {
    return this.http.put(`${this.url}/${item.id}`, item);
  }

  updateStatus(id: number, status: TaskStatus) {
    return this.http.patch(`${this.url}/${id}/status`, { status });
  }

  delete(id: number) {
    return this.http.delete(`${this.url}/${id}`);
  }
}

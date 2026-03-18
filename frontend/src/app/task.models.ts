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

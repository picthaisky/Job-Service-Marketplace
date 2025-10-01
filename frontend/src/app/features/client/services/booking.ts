import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PaginatedResult } from '../../../core/services/api';

export interface Booking {
  id: number;
  clientId: number;
  providerId: number;
  jobTitle: string;
  jobDescription: string;
  scheduledStartDate: Date;
  scheduledEndDate: Date;
  hourlyRate: number;
  estimatedHours: number;
  totalAmount: number;
  status: number;
  acceptedAt?: Date;
  completedAt?: Date;
  createdAt: Date;
}

export interface CreateBooking {
  providerId: number;
  jobTitle: string;
  jobDescription: string;
  scheduledStartDate: Date;
  scheduledEndDate: Date;
  hourlyRate: number;
  estimatedHours: number;
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  constructor(private apiService: ApiService) {}

  getBookings(params?: any): Observable<PaginatedResult<Booking>> {
    return this.apiService.get<PaginatedResult<Booking>>('/bookings', params);
  }

  getBooking(id: number): Observable<Booking> {
    return this.apiService.get<Booking>(`/bookings/${id}`);
  }

  createBooking(data: CreateBooking): Observable<Booking> {
    return this.apiService.post<Booking>('/bookings', data);
  }

  acceptBooking(id: number): Observable<Booking> {
    return this.apiService.post<Booking>(`/bookings/${id}/accept`, {});
  }

  completeBooking(id: number): Observable<Booking> {
    return this.apiService.post<Booking>(`/bookings/${id}/complete`, {});
  }

  cancelBooking(id: number, reason: string): Observable<Booking> {
    return this.apiService.post<Booking>(`/bookings/${id}/cancel`, { cancellationReason: reason });
  }
}

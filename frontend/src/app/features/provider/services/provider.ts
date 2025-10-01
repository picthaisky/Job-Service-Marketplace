import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PaginatedResult } from '../../../core/services/api';

export interface ProviderProfile {
  id: number;
  userId: number;
  profession: string;
  bio: string;
  skills: string;
  hourlyRate: number;
  location: string;
  profileImageUrl?: string;
  isVerified: boolean;
  averageRating: number;
  totalReviews: number;
}

export interface IncomeSummary {
  year: number;
  totalBookings: number;
  totalIncome: number;
  platformCommission: number;
  withholdingTax: number;
  netIncome: number;
}

export interface TaxDocument {
  id: number;
  documentType: string;
  documentNumber: string;
  taxYear: number;
  issuedDate: Date;
  fileUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProviderService {

  constructor(private apiService: ApiService) {}

  getProfile(id: number): Observable<ProviderProfile> {
    return this.apiService.get<ProviderProfile>(`/providers/${id}`);
  }

  getProviders(params?: any): Observable<PaginatedResult<ProviderProfile>> {
    return this.apiService.get<PaginatedResult<ProviderProfile>>('/providers', params);
  }

  createProfile(data: Partial<ProviderProfile>): Observable<ProviderProfile> {
    return this.apiService.post<ProviderProfile>('/providers', data);
  }

  updateProfile(id: number, data: Partial<ProviderProfile>): Observable<ProviderProfile> {
    return this.apiService.put<ProviderProfile>(`/providers/${id}`, data);
  }

  getIncomeSummary(providerId: number, year?: number): Observable<IncomeSummary> {
    const params = year ? { year } : {};
    return this.apiService.get<IncomeSummary>(`/providers/${providerId}/income/summary`, params);
  }

  getTaxDocuments(providerId: number): Observable<TaxDocument[]> {
    return this.apiService.get<TaxDocument[]>(`/providers/${providerId}/tax-documents`);
  }
}

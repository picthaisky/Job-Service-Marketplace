import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ProviderService, ProviderProfile } from '../../../provider/services/provider';

@Component({
  selector: 'app-marketplace',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatPaginatorModule
  ],
  templateUrl: './marketplace.html',
  styleUrl: './marketplace.scss'
})
export class MarketplaceComponent implements OnInit {
  searchForm!: FormGroup;
  providers: ProviderProfile[] = [];
  loading = true;
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;

  professions = [
    'Nurse', 'Electrician', 'Plumber', 'Carpenter',
    'Tutor', 'Cleaner', 'Driver', 'Cook', 'Other'
  ];

  locations = [
    'Bangkok', 'Chiang Mai', 'Phuket', 'Pattaya',
    'Krabi', 'Hua Hin', 'Samui', 'Khon Kaen'
  ];

  constructor(
    private formBuilder: FormBuilder,
    private providerService: ProviderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.searchForm = this.formBuilder.group({
      profession: [''],
      location: [''],
      minRating: ['']
    });

    this.loadProviders();
  }

  loadProviders(): void {
    this.loading = true;
    const params = {
      ...this.searchForm.value,
      page: this.pageIndex + 1,
      pageSize: this.pageSize
    };

    // Remove empty values
    Object.keys(params).forEach(key => {
      if (!params[key]) {
        delete params[key];
      }
    });

    this.providerService.getProviders(params).subscribe({
      next: (result) => {
        this.providers = result.data;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading providers:', error);
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.pageIndex = 0;
    this.loadProviders();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadProviders();
  }

  onReset(): void {
    this.searchForm.reset();
    this.pageIndex = 0;
    this.loadProviders();
  }

  viewProvider(id: number): void {
    this.router.navigate(['/client/provider', id]);
  }

  bookProvider(id: number): void {
    this.router.navigate(['/client/booking'], { queryParams: { providerId: id } });
  }

  getRatingStars(rating: number): number[] {
    return Array(Math.round(rating)).fill(0);
  }
}

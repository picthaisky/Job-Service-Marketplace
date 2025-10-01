import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { ProviderService, IncomeSummary } from '../../services/provider';
import { AuthService } from '../../../../core/services/auth';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    BaseChartDirective
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit {
  incomeSummary?: IncomeSummary;
  loading = true;

  // Chart Configuration
  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: ['Total Income', 'Commission', 'Tax', 'Net Income'],
    datasets: [
      {
        data: [0, 0, 0, 0],
        label: 'Amount (฿)',
        backgroundColor: [
          'rgba(37, 99, 235, 0.6)',
          'rgba(239, 68, 68, 0.6)',
          'rgba(245, 158, 11, 0.6)',
          'rgba(16, 185, 129, 0.6)'
        ],
        borderColor: [
          'rgb(37, 99, 235)',
          'rgb(239, 68, 68)',
          'rgb(245, 158, 11)',
          'rgb(16, 185, 129)'
        ],
        borderWidth: 1
      }
    ]
  };

  public barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false
      },
      title: {
        display: true,
        text: 'Income Breakdown'
      }
    }
  };

  constructor(
    private providerService: ProviderService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadIncomeSummary();
  }

  loadIncomeSummary(): void {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser) return;

    const currentYear = new Date().getFullYear();
    this.providerService.getIncomeSummary(currentUser.id, currentYear).subscribe({
      next: (data) => {
        this.incomeSummary = data;
        this.updateChartData(data);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading income summary:', error);
        this.loading = false;
      }
    });
  }

  updateChartData(data: IncomeSummary): void {
    this.barChartData.datasets[0].data = [
      data.totalIncome,
      data.platformCommission,
      data.withholdingTax,
      data.netIncome
    ];
  }
}

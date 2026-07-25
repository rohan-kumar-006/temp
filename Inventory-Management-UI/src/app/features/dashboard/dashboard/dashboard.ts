import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  
  constructor(
    private authService:AuthService,
    private router:Router
){}

  logout(){

    this.authService.logout();

    this.router.navigate(['/login']);

}
}

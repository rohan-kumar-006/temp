import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/services/auth';

@Component({
  selector: 'app-layout',
  imports: [
        RouterOutlet,
        RouterLink,
        RouterLinkActive
    ],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {
  constructor(
    private authService:AuthService,
    private router:Router
){}

  logout(){
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

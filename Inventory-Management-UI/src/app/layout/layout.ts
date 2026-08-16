import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/services/auth';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-layout',
  imports: [
        RouterOutlet,
        RouterLink,
        RouterLinkActive,
        CommonModule
    ],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {
  constructor(
    private authService:AuthService,
    private router:Router, 
){}

  logout() {
  this.authService.logout().subscribe({
    next: () => {
      this.authService.clearSession();
      this.router.navigate(['/login']);
    },
    error: () => {
      this.authService.clearSession();
      this.router.navigate(['/login']);
    }
  });
}
  isAdmin():boolean{
    return this.authService.isAdmin();
  }
}
// import { Component } from '@angular/core';
// import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
// import { AuthService } from '../core/services/auth';

// @Component({
//   selector: 'app-layout',
//   imports: [
//         RouterOutlet,
//         RouterLink,
//         RouterLinkActive
//     ],
//   templateUrl: './layout.html',
//   styleUrl: './layout.css',
// })
// export class Layout {
//   constructor(
//     private authService:AuthService,
//     private router:Router,
    
// ){}

//   logout(){
//     this.authService.logout();
//     this.router.navigate(['/login']);
//   }
// }

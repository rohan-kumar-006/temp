import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { consumerMarkDirty } from '@angular/core/primitives/signals';
import { FormsModule } from '@angular/forms';

import { AuthService } from '../../../core/services/auth';
import { Router } from '@angular/router';
import { LoginRequest } from '../../../core/models/login-request.model';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  constructor(private authService: AuthService, private router: Router) { }

  email = "";
  password = "";

  login(){

    const request:LoginRequest = {

        email:this.email,

        password:this.password

    };

    this.authService.login(request)
        .subscribe({

            next:(response)=>{

                this.authService.saveToken(response.data.token);

                this.router.navigate(['/dashboard']);

            },

            error:(error)=>{

                alert("Invalid Email or Password");

                console.log(error);

            }

        });

}
}

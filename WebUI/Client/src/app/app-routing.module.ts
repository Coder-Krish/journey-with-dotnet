import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmployeeAddComponent } from './Employees/components/employee-add/employee-add.component';
import { EmployeeViewComponent } from './Employees/components/employee-view/employee-view.component';

const appRoutingConstants = [
  { path: '', redirectTo: 'ViewEmployee', pathMatch: "full" },
  { path: 'ViewEmployee', component: EmployeeViewComponent },
  { path: 'AddEmployee', component: EmployeeAddComponent },
]

@NgModule({
  imports: [RouterModule.forRoot(appRoutingConstants)],
  exports: [RouterModule]
})


export class AppRoutingModule { }

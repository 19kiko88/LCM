import { NgModule } from '@angular/core';
import { MainComponent } from './pages/main/main.component';
import { BsEighteenComponent } from './pages/bs-eighteen/bs-eighteen.component';
import { SharedNgbootstrapModule } from 'src/app/shared/shared-ngbootstrap/shared-ngbootstrap.module';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    MainComponent,
    BsEighteenComponent
  ],
  imports: [
    SharedModule,
    SharedNgbootstrapModule
  ],
  exports: [
    MainComponent, 
    BsEighteenComponent
  ]
})
export class HomeModule { }

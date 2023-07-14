import { NgModule } from '@angular/core';
import { MainComponent } from './pages/main/main.component';
import { BsEighteenComponent } from './pages/bs-eighteen/bs-eighteen.component';
import { SharedNgbootstrapModule } from 'src/app/shared/shared-ngbootstrap/shared-ngbootstrap.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { DatePipe, CommonModule } from '@angular/common';
import { SharedNgMaterialModule } from 'src/app/shared/shared-ngmaterial/shared-ngmaterial.module';
import { ManualCloseComponent } from './pages/manual-close/manual-close.component';

@NgModule({
  declarations: [
    MainComponent,
    BsEighteenComponent,
    ManualCloseComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    SharedNgbootstrapModule,
    SharedNgMaterialModule,        
  ],
  providers: [DatePipe],
  exports: [
    MainComponent, 
    BsEighteenComponent
  ]
})
export class HomeModule { }

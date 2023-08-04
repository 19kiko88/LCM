import { NgModule } from '@angular/core';
import { BsEighteenComponent } from './pages/bs-eighteen/bs-eighteen.component';
import { SharedNgbootstrapModule } from 'src/app/shared/shared-ngbootstrap/shared-ngbootstrap.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { DatePipe, CommonModule } from '@angular/common';
import { SharedNgMaterialModule } from 'src/app/shared/shared-ngmaterial/shared-ngmaterial.module';
import { ManualCloseComponent } from './pages/manual-close/manual-close.component';
import { TabBsComponent } from './pages/tab-bs/tab-bs.component';

@NgModule({
  declarations: [
    BsEighteenComponent,
    ManualCloseComponent,
    TabBsComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    SharedNgbootstrapModule,
    SharedNgMaterialModule,        
  ],
  providers: [DatePipe],
  exports: [
    SharedModule,
    BsEighteenComponent
  ]
})
export class HomeModule { }

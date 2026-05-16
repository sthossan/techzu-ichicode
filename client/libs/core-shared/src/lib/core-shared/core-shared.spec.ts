import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CoreShared } from './core-shared';

describe('CoreShared', () => {
  let component: CoreShared;
  let fixture: ComponentFixture<CoreShared>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoreShared],
    }).compileComponents();

    fixture = TestBed.createComponent(CoreShared);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
